using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    [AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    [UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(2)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(47)] // if != 0, will report failure if expected answer != given answer
    
    partial class Day24
    {
        class SingleHail
        {
            public Vector3 Position { get; set; }
            public Vector3 Velocity { get; set; }
            public override string ToString() => $"{Position} @ {Velocity}";

            // small optimization. we store our line consts
            // instead of computing them each time we compute intersection
            // it's a small optimization, will see the time difference
            // we also normalize our velocity now
            public SingleHail(Vector3 position, Vector3 velocity)
            {
                Position = position;
                Velocity = Vector2.Normalize(velocity);
                A = -Velocity.y;
                B = Velocity.x;
                K = (A * Position.x) + (B * Position.y);
            }
            public double A;
            public double B;
            public double K;
        }

        private static List<SingleHail> ReadHail(string[] input)
        {
            var sw = Stopwatch.StartNew();
            var ret = new List<SingleHail>(input.Length);
            foreach (var line in input)
            {
                var sp = line.Split(new char[] { ',', '@' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(long.Parse).ToArray();
                ret.Add(new SingleHail(new Vector3(sp, 0), new Vector3(sp, 3)));
            }
            sw.Stop();
            Log.WriteLine($"Parsing took {sw.ElapsedMilliseconds}ms ({sw.Elapsed})");
            return ret;
        }
        private static bool LineLineIntersection3D(Vector3 linePoint1, Vector3 lineVel1, Vector3 linePoint2, Vector3 lineVel2, out Vector3 intersection, out bool isHitInPast)
        {
            var lv = linePoint2 - linePoint1;
            var c12 = Vector3.Cross(lineVel1, lineVel2);
            var c32 = Vector3.Cross(lv, lineVel2);

            var factor = Vector3.Dot(lv, c12);

            //is coplanar, and not parallel
            if (Math.Abs(factor) < 0.0001f && c12.sqrMagnitude > 0.0001f)
            {
                var s = Vector3.Dot(c32, c12) / c12.sqrMagnitude;
                intersection = linePoint1 + (lineVel1 * s);
                // technically, we should test for any two of xz, yz, or zy, but I've found 
                // only x is good enough
                isHitInPast = linePoint1.x + lineVel1.x <= linePoint1.x != intersection.x <= linePoint1.x || linePoint2.x + lineVel1.x <= linePoint2.x != intersection.x <= linePoint2.x;
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                isHitInPast = false;
                return false;
            }
        }
        private static bool LineLineIntersection2D(Vector3 linePoint1, Vector3 lineVel1, Vector3 linePoint2, Vector3 lineVel2, out Vector3 intersection, out bool isHitInPast)
        {
            Vector2 l1_pos = new(linePoint1.x, linePoint1.y);
            Vector2 l2_pos = new(linePoint2.x, linePoint2.y);

            // normalize directions. 

            // we know that z is 0 (we are working in 2D space, on z == 0 plane)
            // so normalize and then convert to V2 will work.
            // So I don't have to write Normalize for V2 :)

            // to be 100% correct, this normalization is not required here
            // but it does make sure that any big velocity will get reduced to 
            // have length of 1, which means we won't hit big (point + dir) later

            //lineVel1 = Vector3.Normalize(lineVel1); // I wrote Vect2 normalize in the end, it's not a metric ton of code after all :P
            //lineVel2 = Vector3.Normalize(lineVel2); // so, you can use Vector2.Normalize directly, as bellow, without first going to Vector3

            Vector2 l1_dir = Vector2.Normalize(lineVel1); // there is also an implicit casting operator of Vector2 <-> Vector3, so can use it just like in Unity
            Vector2 l2_dir = Vector2.Normalize(lineVel2);

            // if lines are parallel, we can't hit
            if (l1_dir.IsParallel(l2_dir)

                // if same line, we have infinite amount of solutions, so we elect this to not be the valid answer
                || (l1_pos - l2_pos).IsOrthogonal(l1_dir)
                )
            {
                isHitInPast = false;
                intersection = Vector3.zero;
                return false;
            }


            // make lines expressed as Ax + By = k1 and Cx + Dy = k2

            // you can find solutions for line intersections
            // in this format all over
            // conversion is also well known

            // Normals
            var A = -l1_dir.y;
            var B = l1_dir.x;

            var C = -l2_dir.y;
            var D = l2_dir.x;

            // Ks 
            var k1 = (A * l1_pos.x) + (B * l1_pos.y);
            var k2 = (C * l2_pos.x) + (D * l2_pos.y);

            // Intersection
            var intersectPoint = new Vector2((D * k1 - B * k2) / (A * D - B * C), (-C * k1 + A * k2) / (A * D - B * C));

            // check if this hit was 'behind' any of our positions
            // if so, it 'was in the past'
            isHitInPast = l1_pos.x + l1_dir.x <= l1_pos.x != intersectPoint.x <= l1_pos.x || l2_pos.x + l2_dir.x <= l2_pos.x != intersectPoint.x <= l2_pos.x;

            // convert to V3
            intersection = intersectPoint;
            return true;
        }

        /// <summary>
        /// Thanks to computing line consts and normalization at hail construction,
        /// and to removing of IsParallel() and IsOrthogonal() tests (see inside method)
        /// we cut execution time in half - from 16 to 8 ms for live data.
        /// </summary>
        private static bool LineLineIntersection2D_F(SingleHail h1, SingleHail h2, out Vector3 intersection, out bool isHitInPast)
        {
            Vector2 l1_pos = h1.Position;
            Vector2 l1_dir = h1.Velocity;

            Vector2 l2_pos = h2.Position;
            Vector2 l2_dir = h2.Velocity;

            // if lines are parallel, we can't hit
            
            // note: to test, I removed parallel test.
            // it's relatively expensive, and it appears no lines are parallel in the input
            // so now, only orthogonal test, which is way faster, but let's see if we can skip it as well

            // and yes, orthogonal test can be skipped too.
            // it would appear that no our computations can handle these
            // conditions, and later tests for sign and boundary are faster
            // than calling dot product on vector. Which is strange, but ok..

            if (//l1_dir.IsParallel(l2_dir)
                // if same line, we have infinite amount of solutions, so we elect this to not be the valid answer
                //|| (l1_pos - l2_pos).IsOrthogonal(l1_dir)
            false
                )
            {
                isHitInPast = false;
                intersection = Vector3.zero;
                return false;
            }

            // Normals
            var A = h1.A;
            var B = h1.B;
            var C = h2.A;
            var D = h2.B;
            var k1 = h1.K;
            var k2 = h2.K;

            // Intersection
            var intersectPoint = new Vector2((D * k1 - B * k2) / (A * D - B * C), (-C * k1 + A * k2) / (A * D - B * C));

            // check if this hit was 'behind' any of our positions
            // if so, it 'was in the past'
            isHitInPast = l1_pos.x + l1_dir.x <= l1_pos.x != intersectPoint.x <= l1_pos.x || l2_pos.x + l2_dir.x <= l2_pos.x != intersectPoint.x <= l2_pos.x;

            // convert to V3
            intersection = intersectPoint;
            return true;
        }

        private static bool LineLineIntersection2D_2(Vector3 linePoint1, Vector3 lineVel1, Vector3 linePoint2, Vector3 lineVel2, out Vector3 intersection, out bool isHitInPast)
        {
            lineVel1 = Vector3.Normalize(lineVel1);
            lineVel2 = Vector3.Normalize(lineVel2);

            // technically, this should be faster. but, this seems to be unstablce, and 
            // react to the fact of velocities, which leads me to believe that either I did something wrong
            // or, the some of the numbers are too big and it looses accuracy. This may actually happen,

            var p1 = linePoint1;
            var p2 = linePoint1 + lineVel1;
            var p3 = linePoint2;
            var p4 = linePoint2 + lineVel2;

            var x1 = (decimal)linePoint1.x;
            var y1 = (decimal)linePoint1.y;

            var x2 = (decimal)(linePoint1.x + lineVel1.x);
            var y2 = (decimal)(linePoint1.y + lineVel1.y);

            var x3 = (decimal)linePoint2.x;
            var y3 = (decimal)linePoint2.y;

            var x4 = (decimal)(linePoint2.x + lineVel2.x);
            var y4 = (decimal)(linePoint2.y + lineVel2.y);

            var di = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (Math.Abs(di) == 0) // < double.Epsilon)
            {
                intersection = Vector3.zero;
                isHitInPast = false;
                return false;
            }

            var dex = (((x1 * y2) - (y1 * x2)) * (x3 - x4)) - ((x1 - x2) * ((x3 * y4) - (y3 * x4)));
            var dey = (((x1 * y2) - (y1 * x2)) * (y3 - y4)) - ((x1 - y2) * ((x3 * y4) - (y3 * x4)));

            var dexdi = dex / di;
            var deydi = dey / di;

            var ip = new Vector3((double)dexdi, (double)deydi, 0);

            intersection = ip;
            isHitInPast = x2 <= x1 != dexdi <= x1 || x4 <= x3 != (deydi <= x3);
            return true;
        }

        private static bool GetLineLineIntersection(SingleHail h1, SingleHail h2, out Vector3 point, out bool hitInPast, bool in2dplane, bool use_2 = false, bool useFast = false)
        {
            if (in2dplane)
            {
                // _2 is wrong for some reason, gives bad results.
                // so, we will create both sets and inspect diffs
                if (useFast)
                {
                    return LineLineIntersection2D_F(h1, h2, out point, out hitInPast);
                }
                else if (use_2)
                {
                    return LineLineIntersection2D_2(h1.Position, h1.Velocity, h2.Position, h2.Velocity, out point, out hitInPast);
                }
                else
                {
                    return LineLineIntersection2D(h1.Position, h1.Velocity, h2.Position, h2.Velocity, out point, out hitInPast);
                }
            }
            else
            {
                return LineLineIntersection3D(h1.Position, Vector3.Normalize(h1.Velocity), h2.Position, Vector3.Normalize(h2.Velocity), out point, out hitInPast);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPointInsideBonduaries2D(Vector3 point, Vector3 p1, Vector3 p2) => point.x >= p1.x && point.x <= p2.x && point.y >= p1.y && point.y <= p2.y;

        //public const long TestAreaStartCoordinate = 7;
        //public const long TestAreaEndCoordinate = 27;

        public const long TestAreaStartCoordinate = 200_000_000_000_000;
        public const long TestAreaEndCoordinate = 400_000_000_000_000;

        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string[] input)
        {
            var hail = ReadHail(input);

            var bonduaryP1 = new Vector3(TestAreaStartCoordinate, TestAreaStartCoordinate, 0);
            var bonduaryP2 = new Vector3(TestAreaEndCoordinate, TestAreaEndCoordinate, 0);

            // this is as simple as it gets
            long sum = 0;
            for (int i = 0; i < hail.Count; i++)
            {
                for (int j = i + 1; j < hail.Count; j++)
                {
                    if (GetLineLineIntersection(hail[i], hail[j], out var collPoint, out var hitInPast, in2dplane: true, use_2: false, useFast: true))
                    {
                        if (!hitInPast && IsPointInsideBonduaries2D(collPoint, bonduaryP1, bonduaryP2))
                            sum++;
                    }
                }
            }
            Log.WriteLine($"In total, {hail.Count * (hail.Count-1)} intersections were performed.");
            return sum;
        }
    }

}