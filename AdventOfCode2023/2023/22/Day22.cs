namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(5)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(7)] // if != 0, will report failure if expected answer != given answer
    class Day22
    {
        struct Vector3
        {
            public float x;
            public float y;
            public float z;

            public Vector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
            public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
            public override string ToString() => $"{x}, {y}, {z}";
        }
        public struct Rect
        {
            public float x;
            public float y;
            public float width;
            public float height;
            public float xMin
            {
                get
                {
                    return x;
                }
                set
                {
                    float num = xMax;
                    x = value;
                    width = num - x;
                }
            }
            public float yMin
            {
                get
                {
                    return y;
                }
                set
                {
                    float num = yMax;
                    y = value;
                    width = num - y;
                }
            }
            public float xMax
            {
                get
                {
                    return width + x;
                }
                set
                {
                    width = value - x;
                }
            }
            public float yMax
            {
                get
                {
                    return height + y;
                }
                set
                {
                    height = value - y;
                }
            }

            public Rect(float x, float y, float width, float height)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
            }
            public bool Overlaps(Rect other)
            {
                return other.xMax > xMin && other.xMin < xMax && other.yMax > yMin && other.yMin < yMax;
            }
            public static bool operator !=(Rect lhs, Rect rhs)
            {
                return !(lhs == rhs);
            }
            public static bool operator ==(Rect lhs, Rect rhs)
            {
                return lhs.x == rhs.x && lhs.y == rhs.y && lhs.width == rhs.width && lhs.height == rhs.height;
            }

        }
        class Cube
        {
            public Vector3 BottomLeft;
            public Vector3 Size;
            public string Name;
            public override string ToString()
            {
                return $"{Name}: {BottomLeft}  -  {BottomLeft + Size}  ({Size})";
            }
            public List<Cube> cubesBellow = new();
            public List<Cube> cubesAbove = new();
            public bool IsSupportingCube = false;
            public bool IsFallenCube = true;
            public bool IsSingleSupport(Cube supporter)
                => cubesBellow.Count == 1 && cubesBellow[0] == supporter;
        }


        private static List<Cube> GetCubes(string input)
        {
            // because unity works with reverse coordinate system, 
            // we have to switch y and z
            // this should not affect our computations

            var ret = new List<Cube>();
            var pairs = input.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pair in pairs)
            {
                var sp1 = pair.Split('~', ',').Select(int.Parse).ToArray();
                var v1 = new Vector3(sp1[0], sp1[2], sp1[1]);
                var v2 = new Vector3(sp1[3], sp1[5], sp1[4]);

                // note, we add 1 to our top right coordinate, because, following instructions:
                //>> A line like 2,2,2~2,2,2 means that both ends of the brick are at the same coordinate -
                //>> in other words, that the brick is a single cube.
                Vector3 bl = new(Math.Min(v1.x, v2.x), Math.Min(v1.y, v2.y), Math.Min(v1.z, v2.z));
                Vector3 tr = new Vector3(1, 1, 1) + new Vector3(Math.Max(v1.x, v2.x), Math.Max(v1.y, v2.y), Math.Max(v1.z, v2.z));

                ret.Add(new Cube { BottomLeft = bl, Size = tr - bl, Name = $"{pair}", });
            }
            return ret;
        }
        private static void PutCubesDown(List<Cube> cubes)
        {
            // this method is not the best solution here.
            // what it does is it moves cubes down by n amount
            // this is good for animation (unity)
            // but here, we could just check for down intersections and move to the 
            // first cube as seen from above
            // bellow works, but is considerably slower

            // sort cubes by their y coordinate
            cubes.Sort((a, b) => a.BottomLeft.y.CompareTo(b.BottomLeft.y));

            // note: position 1 is bottom, so we can't fall from that
            var fallenCubes = new List<Cube>();
            for (int i = 0; i < cubes.Count; i++)
            {
                Cube cube = cubes[i];

                while (cube.BottomLeft.y > 1)
                {

                    // check if there are any cubes intersecting with us
                    var ic = GetCubesIntersectingBellow(cube, fallenCubes, out var minDist);

                    if (ic.Count > 0 && minDist < 0.03f)
                    {
                        if (cube.Name.StartsWith("D") || cube.Name.StartsWith("E"))
                        {

                        }
                        // link cube to cube
                        foreach (var _ic in ic)
                        {
                            if (cube.cubesBellow.Contains(_ic) == false)
                                cube.cubesBellow.Add(_ic);

                            if (_ic.cubesAbove.Contains(cube) == false)
                                _ic.cubesAbove.Add(cube);
                        }

                        if (fallenCubes.Contains(cube) == false)
                            fallenCubes.Add(cube);
                        break;
                    }

                    var delta = 0.5f;  // 1m/s
                    cube.BottomLeft.y -= delta;  // 1m/s

                    if (cube.BottomLeft.y < 1)
                        cube.BottomLeft.y = 1;
                }
                if (fallenCubes.Contains(cube) == false)
                    fallenCubes.Add(cube);
            }
        }


        private static bool DoIntersectInXZPlane(Cube a, Cube b)
        {
            // can we cheat here and use unity? we'll do for now, but
            // we may want to rewrite Rect operations later, if we migrate our code
            // to console AoC
            Rect ar = new(a.BottomLeft.x, a.BottomLeft.z, a.Size.x, a.Size.z);
            Rect br = new(b.BottomLeft.x, b.BottomLeft.z, b.Size.x, b.Size.z);

            if (ar == br) return true;
            if (ar.Overlaps(br)) return true;
            if (br.Overlaps(ar)) return true;
            return false;
        }
        private static List<Cube> GetCubesIntersectingBellow(Cube cube, List<Cube> otherCubes, out float minimumFoundDistance)
        {
            minimumFoundDistance = 1;
            var ret = new List<Cube>();
            foreach (var fc in otherCubes)
            {
                // get all cubes that are 
                // a) bellow us
                // b) totally bellow us (together with their size)
                // fallen cubes "should" be bellow us
                // but we need to check for that anyway
                if (fc.BottomLeft.y + fc.Size.y <= cube.BottomLeft.y)
                {
                    if (cube.BottomLeft.y - (fc.BottomLeft.y + fc.Size.y) > 0.1f)
                        continue;

                    minimumFoundDistance = Math.Min(cube.BottomLeft.y - (fc.BottomLeft.y + fc.Size.y), minimumFoundDistance);
                    // now, check if our volume intersects cube volume
                    // if so, this cube is bellow us and we should consider it
                    if (DoIntersectInXZPlane(cube, fc))
                        ret.Add(fc);
                }
            }

            // this is not the best solution, but will do
            return ret;
        }

        private static void ReportPossibleBrickDisintegration(List<Cube> cubes, out int part1, out int part2)
        {
            part1 = 0;
            part2 = 0;

            foreach (var cube in cubes)
            {
                bool canBeRemoved = true;
                // if we have found at least any cube above that is supported by us only, 
                // this brick cannnot be removed.
                foreach (var above in cube.cubesAbove)
                {
                    if (above.cubesBellow.Count == 1 && above.cubesBellow.Contains(cube))
                    {
                        canBeRemoved = false;
                        break;
                    }
                }
                if (canBeRemoved)
                {
                    part1++;
                }
                else
                {
                    // count cubes above us
                    cube.IsSupportingCube = true;
                }
            }
            foreach (var cube in cubes)
            {
                // for each cube, clear this flag before each test
                foreach (var c in cubes) c.IsFallenCube = false;

                //  Debug.Log($"==============================================");

                cube.IsFallenCube = true;
                CheckFallenAbove(cube);
                cube.IsFallenCube = false;

                part2 += cubes.Count(c => c.IsFallenCube);
            }

            //Debug.Log($"Total count of cubes in danger of falling: {sum2}");
        }

        private static void CheckFallenAbove(Cube cube)
        {
            if (cube.IsFallenCube == false) return;

            //Debug.Log($"Cube {cube} fallen");
            // get all above cubes that have only one parent, us
            // and "fall" them
            foreach (var above in cube.cubesAbove)
            {
                // if all cubes bellow are falling, we are too
                if (above.cubesBellow.All(c => c.IsFallenCube))
                {
                    above.IsFallenCube = true;
                }
            }

            foreach (var above in cube.cubesAbove)
            {
                CheckFallenAbove(above);
            }
        }



        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string input, int lineWidth, int count)
        {
            var cubes = GetCubes(input);
            PutCubesDown(cubes);
            ReportPossibleBrickDisintegration(cubes, out var p1, out _);
            return p1;
        }
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(string input, int lineWidth, int count)
        {
            var cubes = GetCubes(input);
            PutCubesDown(cubes);
            ReportPossibleBrickDisintegration(cubes, out var _, out int p2);

            return p2;
        }
    }
}