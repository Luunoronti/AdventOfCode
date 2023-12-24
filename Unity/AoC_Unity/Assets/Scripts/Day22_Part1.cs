using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;

// turned out this is Part 1 and 2 :)
public class Day22_Part1 : MonoBehaviour
{
    public bool useLiveData;
    public string testData;
    public long expectedTestAnswer = 0;
    public int Year = 2023;
    public int Day = 22;
    public bool Animate = true;
    public float animationSpeed = 2;

    public class Cube
    {
        public Vector3 BottomLeft;
        public Vector3 Size;
        public int Index; // we use it for part 2

        public override string ToString() => $"{BottomLeft}  -  {BottomLeft + Size}  ({Size})";
        public Transform transform;

        public List<Cube> cubesBellow = new();
        public List<Cube> cubesAbove = new();
        
    }

    // attempt to perform all calculations in async
    // so important: get UniTask, to ease our job

    async void Start()
    {
        // get input (test is in this object itself, live is being downloaded)
        var input = ReadInput();
        Debug.Log($"Got {input.Length} characters in input.");

        // construct our cubes from the input
        var cubes = GetCubes(input);

        // spawn game objects.
        // this does not work well with live input,
        // over 1k GOs kill performance.
        // if i am going to use unity to visualize,       
        // ECS is the way
        if (Animate)
            SpawnObjects(cubes);

        // apply gravity.
        // this method also animates our spawned cubes
        // on the scene, so it's async, takes time, 
        // and we wait for it
        if (Animate)
            await ApplyGravityAsync(cubes);
        else
            ApplyGravityInstant(cubes);

        // now, analyze and compute final result
        ReportPossibleBrickDisintegration(cubes, out var sum, out var sum2);

        Debug.Log($"Total count of cubes to be safely removed (Part 1): {sum}");
        Debug.Log($"Total count of cubes in danger of falling (Part 2): {sum2}");

    }


    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        
        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        var v = crossVec3and2 * 0.03f;

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parrallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    private string ReadLiveInput()
    {
        
        // we read our live input from a fixed position on C: drive for now
        // I didn't have the time to work on unity framework for AoC yet
        // and I am not sure I'll do it. On the other hand, it may prove
        // to be a good visualization tool, especially with ECS

        // If the file does not exist, create it using Nick Kusters's (https://www.youtube.com/@NKCSS) download code

        var path = $@"C:\temp\aoc\livedata\{Year}_{Day:D0}.txt";
        if (Directory.Exists(path) == false) Directory.CreateDirectory(Path.GetDirectoryName(path));

        if (File.Exists(path)) return File.ReadAllText(path);

        string session = File.ReadAllText($@"C:\temp\aoc\session.txt");
        string url = $"https://adventofcode.com/{Year}/day/{Day}/input";

#pragma warning disable SYSLIB0014 // Type or member is obsolete
        var wc = new WebClient();
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        wc.Headers.Add(HttpRequestHeader.UserAgent, "https://github.com/Luunoronti/AdventOfCode");
        wc.Headers.Add(HttpRequestHeader.Cookie, $"{nameof(session)}={session}");
        string contents = wc.DownloadString(url);

        File.WriteAllText(path, contents);
        return contents;
    }
    private string ReadInput() => useLiveData ? ReadLiveInput() : testData;
    private List<Cube> GetCubes(string input)
    {
        // Because unity works with left-handed coordinate system,
        // and we have our data provided with right-handed system
        // we have to switch y and z. this normally is not enough
        // because we need to reverse y after switch, but we don't
        // work with relations here, only world coordinates.
        // We do this only for our cubes to look naturally in 
        // unity camera.

        // this won't affect our computations

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

            ret.Add(new Cube { BottomLeft = bl, Size = tr - bl, });
        }
        Debug.Log($"Found {ret.Count} cubes.");
        return ret;
    }


    private async UniTask ApplyGravityAsync(List<Cube> cubes)
    {
        // sort cubes by their y coordinate
        cubes.Sort((a, b) => a.BottomLeft.y.CompareTo(b.BottomLeft.y));
        // re-apply indexes after sorting
        for (int i1 = 0; i1 < cubes.Count; i1++)
            cubes[i1].Index = i1;

        // note: position 1 is bottom, so we can't fall from that
        var fallenCubes = new List<Cube>();
        {
            for (int i = 0; i < cubes.Count; i++)
            {
                Cube cube = cubes[i];

                while (cube.BottomLeft.y > 1)
                {
                    var delta = Animate ? (animationSpeed * Time.deltaTime) : 0.5f;  // 1m/s

                    var ic = GetCubesIntersectingBellow(cube, fallenCubes, out var minDist, useSmallRay: false);
                    // simulation problem. we have bigger delta than our minDist
                    // compensate
                    if (minDist < delta)
                    {
                        cube.BottomLeft.y -= minDist;
                    }
                    else
                    {
                        cube.BottomLeft.y -= delta;  // 1m/s
                    }

                    // check if there are any cubes intersecting with us
                    ic = GetCubesIntersectingBellow(cube, fallenCubes, out _);

                    if (ic.Count > 0 && minDist < 0.03f)
                    {
                        // link cube to cube (cube^2 :P)
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

                    if (cube.BottomLeft.y < 1)
                        cube.BottomLeft.y = 1;

                    // because of how async stops under editor,
                    // it may be possible for this condition to be true
                    // that means we quit play mode anyway, so just break
                    if (cube.transform != null)
                    {
                        cube.transform.position =
                            new Vector3(cube.transform.position.x, cube.transform.position.y - delta, cube.transform.position.z);
                    }

                    // animate our cube
                    if (Animate)
                        await UniTask.NextFrame();
                }
                if (fallenCubes.Contains(cube) == false)
                    fallenCubes.Add(cube);
            }


        }
    }
    private void ApplyGravityInstant(List<Cube> cubes)
    {
        // sort cubes by their y coordinate
        cubes.Sort((a, b) => a.BottomLeft.y.CompareTo(b.BottomLeft.y));
        // re-apply indexes after sorting
        for (int i1 = 0; i1 < cubes.Count; i1++)
            cubes[i1].Index = i1;


        // note: position 1 is bottom, so we can't fall from that
        var fallenCubes = new List<Cube>();
        for (int i = 0; i < cubes.Count; i++)
        {
            Cube cube = cubes[i];
            if (cube.BottomLeft.y == 1)
            {
                if (fallenCubes.Contains(cube) == false)
                    fallenCubes.Add(cube);
            }

            // check if there are any cubes intersecting with us
            var ic = GetCubesIntersectingBellow(cube, fallenCubes, out var minDist, useSmallRay: false);

            if (ic.Count == 0)
            {
                // we have nothing bellow us, drop to the bottom
                cube.BottomLeft.y = 1;
                if (fallenCubes.Contains(cube) == false)
                    fallenCubes.Add(cube);
                continue;
            }

            // we have something bellow, move to that position
            cube.BottomLeft.y -= minDist;
            if (cube.BottomLeft.y < 1)
                cube.BottomLeft.y = 1;


            // we must build our list again, but this time, with small ray
            ic = GetCubesIntersectingBellow(cube, fallenCubes, out minDist, useSmallRay: true);

            if (ic.Count > 0)
            {
                foreach (var _ic in ic)
                {
                    if (cube.cubesBellow.Contains(_ic) == false) cube.cubesBellow.Add(_ic);
                    if (_ic.cubesAbove.Contains(cube) == false) _ic.cubesAbove.Add(cube);
                }

                if (fallenCubes.Contains(cube) == false) fallenCubes.Add(cube);
            }
        }
    }

    private static void ReportPossibleBrickDisintegration(List<Cube> cubes, out int part1, out int part2)
    {
        // technically, we could split this
        // into part 1 and part 2, but let's just keep it same as in unity
        part1 = 0;
        part2 = 0;

        Span<bool> fallenFlags = new(new bool[cubes.Count]);

        for (int i = 0; i < cubes.Count; i++)
        {
            Cube cube = cubes[i];

            // part 1
            if (!cube.cubesAbove.Any(a => a.cubesBellow.Count == 1 && a.cubesBellow[0] == cube))
            {
                part1++;

                // because this cube does not cause any other to fall, 
                // it won't count in part 2. so, we can skip that test
                // saves us 
                continue;
            }

            // part 2
            var count = CheckFallenAbove_NR(cube, fallenFlags);
            if (count > 0)
            {
                //cube.WasFallInitiator = true;
            }
            else
            {
                //  Log.WriteLine($"Cube {cube} has not cause any other cube to fall.");
            }
            part2 += count;
        }
    }

    private static bool CheckIfAllSupportMarkedForFall(Cube cube, Span<bool> fallenFlags)
    {
        foreach (var cb in cube.cubesBellow)
        {
            if (!fallenFlags[cb.Index])
                return false;
        }
        return true;
    }


    private static int CheckFallenAbove_NR(Cube startCube, Span<bool> fallenFlags)
    {
        var count = 0;

        Queue<Cube> cubesToCheckAndMark = new();
        fallenFlags.Clear();
        fallenFlags[startCube.Index] = true;

        cubesToCheckAndMark.Enqueue(startCube);
        while (cubesToCheckAndMark.TryDequeue(out var cube))
        {
            foreach (var above in cube.cubesAbove)
            {
                // if checked already, don't queue
                if (fallenFlags[above.Index])
                    continue;

                // check if all support cubes of the cube above are marked for fall or deletion
                if (CheckIfAllSupportMarkedForFall(above, fallenFlags))
                {
                    // mark this cube for fall
                    fallenFlags[above.Index] = true;

                    // add to our overall counter
                    count++;

                    // also, add it to processing
                    cubesToCheckAndMark.Enqueue(above);
                }
            }
        }

        return count;
    }





    private bool DoIntersectInXZPlane(Cube a, Cube b)
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
    private List<Cube> GetCubesIntersectingBellow(Cube cube, List<Cube> otherCubes, out float minimumFoundDistance, bool useSmallRay = true)
    {
        minimumFoundDistance = float.MaxValue;
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
                if (useSmallRay && cube.BottomLeft.y - (fc.BottomLeft.y + fc.Size.y) > 0.5f)
                    continue;

                // now, check if our volume intersects cube volume
                // if so, this cube is bellow us and we should consider it
                if (DoIntersectInXZPlane(cube, fc))
                {
                    minimumFoundDistance = Math.Min(cube.BottomLeft.y - (fc.BottomLeft.y + fc.Size.y), minimumFoundDistance);
                    ret.Add(fc);
                }
            }
        }

        // this is not the best solution, but will do
        return ret;
    }

    private void SpawnObjects(List<Cube> cubes)
    {
        foreach (var c in cubes)
        {
            var worldCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var scale = c.Size;
            worldCube.transform.localScale = scale;
            // note: pivot of the cube in unity is in the center
            // so, to visualize properly, we must move
            worldCube.transform.position = c.BottomLeft + (scale / 2);

            c.transform = worldCube.transform;
        }
    }

}
