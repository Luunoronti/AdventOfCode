using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.XR;

public class Day22_Part1 : MonoBehaviour
{
    public bool useLiveData;
    public string testData;
    public long expectedTestAnswer = 0;
    public int Year = 2023;
    public int Day = 22;
    public bool Animate = true;
    public float animationSpeed = 2;

    class Cube
    {
        public Vector3 BottomLeft;
        public Vector3 Size;
        public string Name;
        public override string ToString()
        {
            return $"{Name}: {BottomLeft}  -  {BottomLeft + Size}  ({Size})";
        }
        public Transform transform;

        public List<Cube> cubesBellow = new();
        public List<Cube> cubesAbove = new();

        public bool IsSupportingCube = false;
        public bool IsFallenCube = true;

        public bool IsSingleSupport(Cube supporter)
            => cubesBellow.Count == 1 && cubesBellow[0] == supporter;

    }

    // attempt to perform all calculations in async
    // so important: get UniTask, to ease our job

    async void Start()
    {
        var input = ReadInput();
        Debug.Log($"Got {input.Length} characters in input.");
        var cubes = GetCubes(input);

        if (Animate)
            SpawnObjects(cubes);

        await PutCubesDown(cubes);

        ReportPossibleBrickDisintegration(cubes);
    }







    private string ReadLiveInput()
    {
        // we read our live input from a fixed position on C: drive for now
        // i didn't have the time to work with unity framework yet

        // but, if file does not exist, create it using Nick's download code
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
        // because unity works with reverse coordinate system, 
        // we have to switch y and z
        // this should not affect our computations

        var ret = new List<Cube>();
        var pairs = input.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        char ch = 'A';
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

            ret.Add(new Cube { BottomLeft = bl, Size = tr - bl, Name = $"{ch} {pair}", });
            if (!useLiveData)
                ch = ++ch;
            if (ch > 'Z') ch = 'Z';
        }
        Debug.Log($"Found {ret.Count} cubes.");
        return ret;
    }

    private async UniTask PutCubesDown(List<Cube> cubes)
    {
        // sort cubes by their y coordinate
        cubes.Sort((a, b) => a.BottomLeft.y.CompareTo(b.BottomLeft.y));

        // note: position 1 is bottom, so we can't fall from that
        var fallenCubes = new List<Cube>();
        for (int i = 0; i < cubes.Count; i++)
        {
            Cube cube = cubes[i];
            Debug.Log($"Processing cube {i} / {cubes.Count}");

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

                var delta = Animate ? (animationSpeed * Time.deltaTime) : 0.5f;  // 1m/s
                cube.BottomLeft.y -= delta;  // 1m/s

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

    private void ReportPossibleBrickDisintegration(List<Cube> cubes)
    {
        var sum = 0;
        var sum2 = 0;
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
                Debug.Log($"Cube can be removed: [{cube}]");
                sum++;
            }
            else
            {
                // count cubes above us
                cube.IsSupportingCube = true;
                Debug.LogWarning($"Cube can not be removed: [{cube}]");
            }
        }
        Debug.Log($"Total count of cubes to be safely removed: {sum}");

        // 714738 too high
        foreach (var cube in cubes)
        {
            // for each cube, clear this flag before each test
            foreach (var c in cubes) c.IsFallenCube = false;

          //  Debug.Log($"==============================================");

            cube.IsFallenCube = true;
            CheckFallenAbove(cube);
            cube.IsFallenCube = false;

            sum2 += cubes.Count(c => c.IsFallenCube);
        }



        Debug.Log($"Total count of cubes in danger of falling: {sum2}");
    }

    private void CheckFallenAbove(Cube cube)
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
    private List<Cube> GetCubesIntersectingBellow(Cube cube, List<Cube> otherCubes, out float minimumFoundDistance)
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

            var note = worldCube.AddComponent<ObjectNoteInGame>();
            note.NoteText = c.Name;
            note.ShowWhenSelected = true;
            note.ShowWhenUnselected = true;
            note.ShowInGameEditor = true;
            note.Alignment = TextAlignment.Left;
            note.FontSize = 20;

            worldCube.name = c.Name;

            c.transform = worldCube.transform;
        }
    }

}
