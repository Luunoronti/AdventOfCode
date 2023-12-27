namespace AdventOfCode2015
{
    [Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    [UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(0)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
    [RequestsVisualizer]
    class Day07
    {
        private static int __globalId = 0;
        enum GateType
        {
            And,
            Or,
            LShift,
            RShift,
            Not,
            Move,
            ValueSource,
            Destination
        }

        class Wire
        {
            public string Name;
            public ushort Value; // in bits, we could use short, but.. whatever :P
            public bool hasSignal;
            public void Set(ushort value)
            {
                Value = value;
                hasSignal = true;
            }
            public override string ToString() => $"{Name,3}: {Value,5} [ {Convert.ToString(Value, 2),16} ]";
        }
        class Source
        {
            public string Name;
            public Wire wireSource;
            public ushort scalarSource;
            public ushort Value => wireSource?.Value ?? scalarSource;
            public bool HasSignal => wireSource?.hasSignal ?? true;
            public override string ToString() => $"{Name,3}: {Value,5} [ {Convert.ToString(Value, 2),16} ]";

        }

        class Gate
        {
            public Gate() => Id = ++__globalId;
            public int Id;
            public GateType Type;
            public Source Source1;
            public Source Source2;

            public Wire Destination;

            public bool processed = false;


            public bool Process()
            {
                if (processed) return false;

                // LSH   c:     0 [                0 ] < 1 =>   t:     0 [                0 ]

                switch (Type)
                {
                    case GateType.LShift:
                        if (Source1.HasSignal == false) return false;
                        Destination.Set((ushort)(Source1.Value << Source2.Value));
                        Log.WriteLine($"LSH {CC.Val}{Source1}{CC.Clr} < {CC.Att}{Source2}{CC.Clr} => {CC.Sys}{Destination}{CC.Clr}");
                        processed = true;
                        return true;

                    case GateType.RShift:
                        if (Source1.HasSignal == false) return false;
                        Destination.Set((ushort)(Source1.Value >> Source2.Value));
                        Log.WriteLine($"RSH {CC.Val}{Source1}{CC.Clr} > {CC.Att}{Source2}{CC.Clr} => {CC.Sys}{Destination}{CC.Clr}");
                        processed = true;
                        return true;

                    case GateType.And:
                        if (Source1.HasSignal == false || Source2.HasSignal == false) return false;
                        Destination.Set((ushort)(Source1.Value & Source2.Value));
                        Log.WriteLine($"AND {CC.Val}{Source1}{CC.Clr} & {CC.Att}{Source2}{CC.Clr} => {CC.Sys}{Destination}{CC.Clr}");
                        processed = true;
                        return true;

                    case GateType.Or:
                        if (Source1.HasSignal == false || Source2.HasSignal == false) return false;
                        Destination.Set((ushort)(Source1.Value | Source2.Value));
                        Log.WriteLine($" OR {CC.Val}{Source1}{CC.Clr} | {CC.Att}{Source2}{CC.Clr} => {CC.Sys}{Destination}{CC.Clr}");
                        processed = true;
                        return true;

                    case GateType.Not:
                        if (Source1.HasSignal == false) return false;
                        Destination.Set((ushort)(~Source1.Value));
                        Log.WriteLine($"NOT {CC.Val}{Source1}{CC.Clr} ! {"",31} => {CC.Sys}{Destination}{CC.Clr}");
                        processed = true;
                        return true;
                    case GateType.Move:
                        if (Source1.HasSignal == false) return false;
                        Destination.Set(Source1.Value);
                        Log.WriteLine($"MOV {CC.Val}{Source1}{CC.Clr} => {"",30} => {CC.Sys}{Destination}{CC.Clr}");
                        processed = true;
                        return true;
                }
                return false;
            }
        }


        class Edge
        {
            public string Name;
            public Node Source;
            public List<Node> Destinations = new ();
            public ushort Value;
            public bool IsValueLocked;

            public void Set(ushort value)
            {
                Value = value;
                IsValueLocked = true;
            }
        }
        class Node
        {
            public int Id;
            public GateType Type;
            public ushort Value;
            public bool processed = false;
            public Edge Input1;
            public Edge Input2;
            public Edge Output;

            public bool Process()
            {
                if (processed) return false;

                switch (Type)
                {
                    case GateType.Destination:
                        processed = true;
                        return true;

                    case GateType.LShift:
                        if (Input1.IsValueLocked == false || Input2.IsValueLocked == false) return false;
                        Output.Set((ushort)(Input1.Value << Input2.Value));
                        processed = true;
                        return true;
                    case GateType.RShift:
                        if (Input1.IsValueLocked == false || Input2.IsValueLocked == false) return false;
                        Output.Set((ushort)(Input1.Value >> Input2.Value));
                        processed = true;
                        return true;
                    case GateType.And:
                        if (Input1.IsValueLocked == false || Input2.IsValueLocked == false) return false;
                        Output.Set((ushort)(Input1.Value & Input2.Value));
                        processed = true;
                        return true;
                    case GateType.Or:
                        if (Input1.IsValueLocked == false || Input2.IsValueLocked == false) return false;
                        Output.Set((ushort)(Input1.Value | Input2.Value));
                        processed = true;
                        return true;
                    case GateType.Not:
                        if (Input1.IsValueLocked == false) return false;
                        Output.Set((ushort)(~Input1.Value));
                        processed = true;
                        return true;
                    case GateType.Move:
                        if (Input1.IsValueLocked == false) return false;
                        Output.Set(Input1.Value);
                        processed = true;
                        return true;
                    case GateType.ValueSource:
                        Output.Set(Value);
                        processed = true;
                        return true;
                }
                return true;
            }

        }



        class Device
        {
            public List<Node> nodes = new();
            public List<Edge> edges = new();

            public Edge GetEdge(string name)
            {
                if (ushort.TryParse(name, out var val))
                {
                    var vedge = new Edge
                    {
                        Name = name,
                    };
                    var node = new Node
                    {
                        Id = ++__globalId,
                        Type = GateType.ValueSource,
                        Value = val,
                        Output = vedge,
                    };
                    nodes.Add(node);
                    edges.Add(vedge);
                    vedge.Source = node;

                    return vedge;
                }

                var ee = edges.FirstOrDefault(e => e.Name == name);
                if (ee != null) return ee;

                var edge = new Edge
                {
                    Name = name,
                };
                edges.Add(edge);
                return edge;
            }
            public Device(string[] input)
            {
                // put our machine together
                foreach (var l in input)
                {
                    var s1 = l.Split("->", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    if (s1[0].Contains(" AND "))
                    {
                        var ss = s1[0].Split(" AND ");

                        var i1 = GetEdge(ss[0]);
                        var i2 = GetEdge(ss[1]);
                        var o1 = GetEdge(s1[1]);

                        var n = new Node
                        {
                            Id = ++__globalId,
                            Type = GateType.And,
                            Input1 = i1,
                            Input2 = i2,
                            Output = o1,
                        };

                        i1.Destinations.Add(n);
                        i2.Destinations.Add(n);
                        o1.Source = n;

                        nodes.Add(n);
                    }
                    else if (s1[0].Contains(" OR "))
                    {
                        var ss = s1[0].Split(" OR ");

                        var i1 = GetEdge(ss[0]);
                        var i2 = GetEdge(ss[1]);
                        var o1 = GetEdge(s1[1]);

                        var n = new Node
                        {
                            Id = ++__globalId,
                            Type = GateType.Or,
                            Input1 = i1,
                            Input2 = i2,
                            Output = o1,
                        };

                        i1.Destinations.Add(n);
                        i2.Destinations.Add(n);
                        o1.Source = n;

                        nodes.Add(n);
                    }
                    else if (s1[0].Contains(" LSHIFT "))
                    {
                        var ss = s1[0].Split(" LSHIFT ");

                        var i1 = GetEdge(ss[0]);
                        var i2 = GetEdge(ss[1]);
                        var o1 = GetEdge(s1[1]);

                        var n = new Node
                        {
                            Id = ++__globalId,
                            Type = GateType.LShift,
                            Input1 = i1,
                            Input2 = i2,
                            Output = o1,
                        };

                        i1.Destinations.Add(n);
                        i2.Destinations.Add(n);
                        o1.Source = n;

                        nodes.Add(n);
                    }
                    else if (s1[0].Contains(" RSHIFT "))
                    {
                        var ss = s1[0].Split(" RSHIFT ");

                        var i1 = GetEdge(ss[0]);
                        var i2 = GetEdge(ss[1]);
                        var o1 = GetEdge(s1[1]);

                        var n = new Node
                        {
                            Id = ++__globalId,
                            Type = GateType.RShift,
                            Input1 = i1,
                            Input2 = i2,
                            Output = o1,
                        };

                        i1.Destinations.Add(n);
                        i2.Destinations.Add(n);
                        o1.Source = n;

                        nodes.Add(n);
                    }
                    else if (s1[0].StartsWith("NOT "))
                    {
                        var i1 = GetEdge(s1[0][4..]);
                        var o1 = GetEdge(s1[1]);
                        var n = new Node
                        {
                            Id = ++__globalId,
                            Type = GateType.Not,
                            Input1 = i1,
                            Output = o1,
                        };

                        i1.Destinations.Add(n);
                        o1.Source = n;
                        nodes.Add(n);
                    }
                    else
                    {
                        if (ushort.TryParse(s1[0], out var val))
                        {
                            var o1 = GetEdge(s1[1]);
                            var n = new Node
                            {
                                Id = ++__globalId,
                                Type = GateType.ValueSource,
                                Output = o1,
                                Value = val,
                            };

                            o1.Source = n;
                            nodes.Add(n);
                        }
                        else
                        {
                            var i1 = GetEdge(s1[0]);
                            var o1 = GetEdge(s1[1]);
                            var n = new Node
                            {
                                Id = ++__globalId,
                                Type = GateType.Move,
                                Input1 = i1,
                                Output = o1,
                            };

                            i1.Destinations.Add(n);
                            o1.Source = n;
                            nodes.Add(n);
                        }
                    }
                }

                foreach (var edge in edges)
                {
                    if (edge.Destinations.Count == 0)
                    {
                        var node = new Node { Id = ++__globalId, Input1 = edge, Type = GateType.Destination };
                        edge.Destinations.Add(node);
                        nodes.Add(node);
                    }
                }

            }

            public bool Process()
            {
                var processAny = false;
                int count = 0;
                foreach (var gate in nodes)
                {
                    if (gate.Process())
                    {
                        processAny = true;
                        count++;
                    }
                }
                Console.CursorLeft = 0;
                Log.Write($"Gates processed: {count,5} / {nodes.Count}");
                return processAny;
            }

            public void SendDiagram()
            {
                DiagramContext dc = new DiagramContext();

                foreach (var node in nodes)
                {
                    if (node.Type == GateType.ValueSource)
                        dc.AddNode(node.Id, $"[ {node.Value} >>>", 0, 0, 80, 30, 1);
                    else if (node.Type == GateType.Destination)
                        dc.AddNode(node.Id, $" >>> {node.Input1.Name} ({node.Value}) ]", 0, 0, 120, 30, 1);
                    else
                        dc.AddNode(node.Id, node.Type.ToString(), 0, 0, 80, 30, 1);
                }

                foreach (var edge in edges)
                {
                    //if (edge.Source != null && edge.Destination != null)
                    foreach (var dst in edge.Destinations)
                    {
                        dc.AddConnector(edge.Source?.Id ?? -1, dst?.Id ?? -1, edge.Name);
                    }
                }
                dc.Send();
            }


        }

        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string[] input)
        {
            var device = new Device(input);

            device.SendDiagram();

            while (true)
            {
                //Log.WriteLine("\n==========================");
                //device.PrintWireValues();
                //Log.WriteLine("-------------------------");
                if (!device.Process())
                    break;
                Console.ReadLine();
            }

            //device.PrintNEGates();
            //device.PrintWireValues();
            return 0;// device.GetOrAdd("a").Value;
        }
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(string[] input)
        {
            return 0;
        }
    }
}



/*
 
Gates processed:     0 / 337fbn RSH =>  bo:     0 [                0 ]
 lf RSH =>  ly:     0 [                0 ]
 fo RSH =>  fq:     0 [                0 ]
 cj  OR  cp =>  cq:     0 [                0 ]
 fo  OR  fz =>  ga:     0 [                0 ]
 lx MOV =>   a:     0 [                0 ]
 he RSH =>  hf:     0 [                0 ]
 lf  OR  lq =>  lr:     0 [                0 ]
 lr AND  lt =>  lu:     0 [                0 ]
 dy  OR  ej =>  ek:     0 [                0 ]
SCL AND  cx =>  cy:     0 [                0 ]
 hb LSH =>  hv:     0 [                0 ]
 ih AND  ij =>  ik:     0 [                0 ]
 ea AND  eb =>  ed:     0 [                0 ]
 km  OR  kn =>  ko:     0 [                0 ]
 bw NOT =>  bx:     0 [                0 ]
 ci  OR  ct =>  cu:     0 [                0 ]
 lw  OR  lv =>  lx:     0 [                0 ]
 lo NOT =>  lp:     0 [                0 ]
 fp  OR  fv =>  fw:     0 [                0 ]
 dh AND  dj =>  dk:     0 [                0 ]
 ii NOT =>  ij:     0 [                0 ]
 gh  OR  gi =>  gj:     0 [                0 ]
 kk RSH =>  ld:     0 [                0 ]
 lc LSH =>  lw:     0 [                0 ]
 lb  OR  la =>  lc:     0 [                0 ]
 gn AND  gp =>  gq:     0 [                0 ]
 lf RSH =>  lh:     0 [                0 ]
 lg AND  lm =>  lo:     0 [                0 ]
 ci RSH =>  db:     0 [                0 ]
 cf LSH =>  cz:     0 [                0 ]
 et AND  fe =>  fg:     0 [                0 ]
 is  OR  it =>  iu:     0 [                0 ]
 kw AND  ky =>  kz:     0 [                0 ]
 ck AND  cl =>  cn:     0 [                0 ]
 gj RSH =>  hc:     0 [                0 ]
 iu AND  jf =>  jh:     0 [                0 ]
 kk  OR  kv =>  kw:     0 [                0 ]
 ks AND  ku =>  kv:     0 [                0 ]
 hz  OR  ik =>  il:     0 [                0 ]
 iu RSH =>  jn:     0 [                0 ]
 fo RSH =>  fr:     0 [                0 ]
 ga AND  gc =>  gd:     0 [                0 ]
 hf  OR  hl =>  hm:     0 [                0 ]
 ld  OR  le =>  lf:     0 [                0 ]
 fm  OR  fn =>  fo:     0 [                0 ]
 hm AND  ho =>  hp:     0 [                0 ]
 lg  OR  lm =>  ln:     0 [                0 ]
 kx NOT =>  ky:     0 [                0 ]
 kk RSH =>  km:     0 [                0 ]
 ek AND  em =>  en:     0 [                0 ]
 ft NOT =>  fu:     0 [                0 ]
 jh NOT =>  ji:     0 [                0 ]
 jn  OR  jo =>  jp:     0 [                0 ]
 gj AND  gu =>  gw:     0 [                0 ]
 et RSH =>  fm:     0 [                0 ]
 jq  OR  jw =>  jx:     0 [                0 ]
 ep  OR  eo =>  eq:     0 [                0 ]
 lv LSH =>  lz:     0 [                0 ]
 ey NOT =>  ez:     0 [                0 ]
 jp RSH =>  jq:     0 [                0 ]
 eg AND  ei =>  ej:     0 [                0 ]
 dm NOT =>  dn:     0 [                0 ]
 jp AND  ka =>  kc:     0 [                0 ]
 fk  OR  fj =>  fl:     0 [                0 ]
 dw  OR  dx =>  dy:     0 [                0 ]
 lj AND  ll =>  lm:     0 [                0 ]
 ec AND  ee =>  ef:     0 [                0 ]
 fq AND  fr =>  ft:     0 [                0 ]
 kp NOT =>  kq:     0 [                0 ]
 ki  OR  kj =>  kk:     0 [                0 ]
 cz  OR  cy =>  da:     0 [                0 ]
 fj LSH =>  fn:     0 [                0 ]
SCL AND  fi =>  fj:     0 [                0 ]
 he RSH =>  hx:     0 [                0 ]
 lf RSH =>  lg:     0 [                0 ]
 kf LSH =>  kj:     0 [                0 ]
 dz AND  ef =>  eh:     0 [                0 ]
 ib  OR  ic =>  id:     0 [                0 ]
 lf RSH =>  li:     0 [                0 ]
 gs NOT =>  gt:     0 [                0 ]
 fo RSH =>  gh:     0 [                0 ]
 bz AND  cb =>  cc:     0 [                0 ]
 ea  OR  eb =>  ec:     0 [                0 ]
 lf AND  lq =>  ls:     0 [                0 ]
 hz RSH =>  ib:     0 [                0 ]
 di NOT =>  dj:     0 [                0 ]
 lk NOT =>  ll:     0 [                0 ]
 jp RSH =>  jr:     0 [                0 ]
 jp RSH =>  js:     0 [                0 ]
 eq LSH =>  fk:     0 [                0 ]
 jl  OR  jk =>  jm:     0 [                0 ]
 hz AND  ik =>  im:     0 [                0 ]
 dz  OR  ef =>  eg:     0 [                0 ]
SCL AND  gy =>  gz:     0 [                0 ]
 la LSH =>  le:     0 [                0 ]
 cn NOT =>  co:     0 [                0 ]
SCL AND  gd =>  ge:     0 [                0 ]
 ia  OR  ig =>  ih:     0 [                0 ]
 go NOT =>  gp:     0 [                0 ]
 ed NOT =>  ee:     0 [                0 ]
 jq AND  jw =>  jy:     0 [                0 ]
 et  OR  fe =>  ff:     0 [                0 ]
 ff AND  fh =>  fi:     0 [                0 ]
 ir LSH =>  jl:     0 [                0 ]
 gg LSH =>  ha:     0 [                0 ]
 db  OR  dc =>  dd:     0 [                0 ]
 ib AND  ic =>  ie:     0 [                0 ]
 lh AND  li =>  lk:     0 [                0 ]
 ce  OR  cd =>  cf:     0 [                0 ]
 hi AND  hk =>  hl:     0 [                0 ]
 gb NOT =>  gc:     0 [                0 ]
 fw AND  fy =>  fz:     0 [                0 ]
 fb AND  fd =>  fe:     0 [                0 ]
SCL AND  en =>  eo:     0 [                0 ]
 hg  OR  hh =>  hi:     0 [                0 ]
 kh LSH =>  lb:     0 [                0 ]
 cg  OR  ch =>  ci:     0 [                0 ]
SCL AND  kz =>  la:     0 [                0 ]
 gf  OR  ge =>  gg:     0 [                0 ]
 gj RSH =>  gk:     0 [                0 ]
 dd RSH =>  de:     0 [                0 ]
 ls NOT =>  lt:     0 [                0 ]
 lh  OR  li =>  lj:     0 [                0 ]
 jr  OR  js =>  jt:     0 [                0 ]
 he AND  hp =>  hr:     0 [                0 ]
 id AND  if =>  ig:     0 [                0 ]
 et RSH =>  ew:     0 [                0 ]
 ly  OR  lz =>  ma:     0 [                0 ]
SCL AND  lu =>  lv:     0 [                0 ]
 jd NOT =>  je:     0 [                0 ]
 ha  OR  gz =>  hb:     0 [                0 ]
 dy RSH =>  er:     0 [                0 ]
 iu RSH =>  iv:     0 [                0 ]
 hr NOT =>  hs:     0 [                0 ]
 kk RSH =>  kl:     0 [                0 ]
 ln AND  lp =>  lq:     0 [                0 ]
 cj AND  cp =>  cr:     0 [                0 ]
 dl AND  dn =>  do:     0 [                0 ]
 ci RSH =>  cj:     0 [                0 ]
 ge LSH =>  gi:     0 [                0 ]
 hz RSH =>  ic:     0 [                0 ]
 dv LSH =>  ep:     0 [                0 ]
 kl  OR  kr =>  ks:     0 [                0 ]
 gj  OR  gu =>  gv:     0 [                0 ]
 he RSH =>  hh:     0 [                0 ]
 fg NOT =>  fh:     0 [                0 ]
 hg AND  hh =>  hj:     0 [                0 ]
 jk LSH =>  jo:     0 [                0 ]
 gz LSH =>  hd:     0 [                0 ]
 cy LSH =>  dc:     0 [                0 ]
 kk RSH =>  kn:     0 [                0 ]
 ci RSH =>  ck:     0 [                0 ]
 iu RSH =>  iw:     0 [                0 ]
 ko AND  kq =>  kr:     0 [                0 ]
 eh NOT =>  ei:     0 [                0 ]
 iy AND  ja =>  jb:     0 [                0 ]
 dd RSH =>  df:     0 [                0 ]
SCL AND  cc =>  cd:     0 [                0 ]
 kk AND  kv =>  kx:     0 [                0 ]
 dy RSH =>  ea:     0 [                0 ]
 eu AND  fa =>  fc:     0 [                0 ]
 kl AND  kr =>  kt:     0 [                0 ]
 ia AND  ig =>  ii:     0 [                0 ]
 df AND  dg =>  di:     0 [                0 ]
 fx NOT =>  fy:     0 [                0 ]
 km AND  kn =>  kp:     0 [                0 ]
 dt LSH =>  dx:     0 [                0 ]
 hz RSH =>  ia:     0 [                0 ]
 cd LSH =>  ch:     0 [                0 ]
 hc  OR  hd =>  he:     0 [                0 ]
 he RSH =>  hg:     0 [                0 ]
 bn  OR  by =>  bz:     0 [                0 ]
 kt NOT =>  ku:     0 [                0 ]
 cu AND  cw =>  cx:     0 [                0 ]
 ie NOT =>  if:     0 [                0 ]
 dy RSH =>  dz:     0 [                0 ]
 ip LSH =>  it:     0 [                0 ]
 de  OR  dk =>  dl:     0 [                0 ]
 jg AND  ji =>  jj:     0 [                0 ]
 ci AND  ct =>  cv:     0 [                0 ]
 dy RSH =>  eb:     0 [                0 ]
 hx  OR  hy =>  hz:     0 [                0 ]
 eu  OR  fa =>  fb:     0 [                0 ]
 gj RSH =>  gl:     0 [                0 ]
 fo AND  fz =>  gb:     0 [                0 ]
SCL AND  jj =>  jk:     0 [                0 ]
 jp  OR  ka =>  kb:     0 [                0 ]
 de AND  dk =>  dm:     0 [                0 ]
 ex AND  ez =>  fa:     0 [                0 ]
 df  OR  dg =>  dh:     0 [                0 ]
 iv  OR  jb =>  jc:     0 [                0 ]
 hj NOT =>  hk:     0 [                0 ]
 im NOT =>  in:     0 [                0 ]
 fl LSH =>  gf:     0 [                0 ]
 hu LSH =>  hy:     0 [                0 ]
 iq  OR  ip =>  ir:     0 [                0 ]
 iu RSH =>  ix:     0 [                0 ]
 fc NOT =>  fd:     0 [                0 ]
 el NOT =>  em:     0 [                0 ]
 ck  OR  cl =>  cm:     0 [                0 ]
 et RSH =>  ev:     0 [                0 ]
 hw LSH =>  iq:     0 [                0 ]
 ci RSH =>  cl:     0 [                0 ]
 iv AND  jb =>  jd:     0 [                0 ]
 dd RSH =>  dg:     0 [                0 ]
 jy NOT =>  jz:     0 [                0 ]
SCL AND  ds =>  dt:     0 [                0 ]
 jx AND  jz =>  ka:     0 [                0 ]
 da LSH =>  du:     0 [                0 ]
 fs AND  fu =>  fv:     0 [                0 ]
 jp RSH =>  ki:     0 [                0 ]
 iw AND  ix =>  iz:     0 [                0 ]
 iw  OR  ix =>  iy:     0 [                0 ]
 eo LSH =>  es:     0 [                0 ]
 ev AND  ew =>  ey:     0 [                0 ]
 fp AND  fv =>  fx:     0 [                0 ]
 jc AND  je =>  jf:     0 [                0 ]
 et RSH =>  eu:     0 [                0 ]
 kg  OR  kf =>  kh:     0 [                0 ]
 iu  OR  jf =>  jg:     0 [                0 ]
 er  OR  es =>  et:     0 [                0 ]
 fo RSH =>  fp:     0 [                0 ]
 ca NOT =>  cb:     0 [                0 ]
 bv AND  bx =>  by:     0 [                0 ]
 cm AND  co =>  cp:     0 [                0 ]
 bn AND  by =>  ca:     0 [                0 ]
SCL AND  ke =>  kf:     0 [                0 ]
 jt AND  jv =>  jw:     0 [                0 ]
 fq  OR  fr =>  fs:     0 [                0 ]
 dy AND  ej =>  el:     0 [                0 ]
 kc NOT =>  kd:     0 [                0 ]
 ev  OR  ew =>  ex:     0 [                0 ]
 dd  OR  do =>  dp:     0 [                0 ]
 cv NOT =>  cw:     0 [                0 ]
 gr AND  gt =>  gu:     0 [                0 ]
 dd RSH =>  dw:     0 [                0 ]
 gw NOT =>  gx:     0 [                0 ]
 iz NOT =>  ja:     0 [                0 ]
SCL AND  io =>  ip:     0 [                0 ]
 cr NOT =>  cs:     0 [                0 ]
 kb AND  kd =>  ke:     0 [                0 ]
 jr AND  js =>  ju:     0 [                0 ]
 cq AND  cs =>  ct:     0 [                0 ]
 il AND  in =>  io:     0 [                0 ]
 ju NOT =>  jv:     0 [                0 ]
 du  OR  dt =>  dv:     0 [                0 ]
 dd AND  do =>  dq:     0 [                0 ]
 jm LSH =>  kg:     0 [                0 ]
 dq NOT =>  dr:     0 [                0 ]
 bo  OR  bu =>  bv:     0 [                0 ]
 gk  OR  gq =>  gr:     0 [                0 ]
 he  OR  hp =>  hq:     0 [                0 ]
 hf AND  hl =>  hn:     0 [                0 ]
 gv AND  gx =>  gy:     0 [                0 ]
 bo AND  bu =>  bw:     0 [                0 ]
 hq AND  hs =>  ht:     0 [                0 ]
 hz RSH =>  is:     0 [                0 ]
 gj RSH =>  gm:     0 [                0 ]
 gk AND  gq =>  gs:     0 [                0 ]
 dp AND  dr =>  ds:     0 [                0 ]
 gl AND  gm =>  go:     0 [                0 ]
 gl  OR  gm =>  gn:     0 [                0 ]
 hv  OR  hu =>  hw:     0 [                0 ]
SCL AND  ht =>  hu:     0 [                0 ]
 hn NOT =>  ho:     0 [                0 ] 
 
 */