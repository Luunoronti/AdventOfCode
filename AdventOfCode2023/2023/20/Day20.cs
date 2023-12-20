using System.Reflection;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    [AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    [UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(11687500)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
    class Day20
    {
        // steps:
        // define all possible modules as classes, deriving from Module class
        // define a Device class that will hold all modules
        // Device 

        [AttributeUsage(AttributeTargets.Class)] class PrefixAttribute : Attribute { public PrefixAttribute(char prefix) { Prefix = prefix; } public char Prefix { get; set; } }
        private static char[] StdSplitChars = new char[] { ',' };
        private static string DevSplitString = "->";
        private const string ButtonModuleName = "button";
        private const string OutputModuleName = "output";
        private const string BroadcastModuleName = "broadcast";
        private const string RxModuleName = "rx";

        private const bool HIGH = true;
        private const bool LOW = false; 

        class Pulse
        {
            private string _from;
            private string _to;
            private bool _isHighPulse;

            public string From => _from;
            public string To => _to;
            public bool IsHighPulse => _isHighPulse;

            public Pulse(string from, string to, bool isHighPulse)
            {
                _from = from;
                _to = to;
                _isHighPulse = isHighPulse;
            }
        }

        private class Device
        {
            private static Device _device;
            public static Device TheDevice
            {
                get
                {
                    _device ??= new Device();
                    return _device;
                }
            }

            private Queue<Pulse> _queuedPulses = new();
            private Dictionary<string, Module> _modules = new();

            public long HighLevelPulsesCount { get; private set; }
            public long LowLevelPulsesCount { get; private set; }

            public bool HasModule(string name) => _modules.ContainsKey(name);
            public Module GetModule(string name) => _modules.TryGetValue(name, out var dev) ? dev : null;
            public IReadOnlyList<Module> GetAllModules() => _modules.Values.ToList();
            public void SendPulse(Pulse pulse)
            {
                if (pulse.IsHighPulse) HighLevelPulsesCount++;
                else LowLevelPulsesCount++;
                _queuedPulses.Enqueue(pulse);
            }
            public bool Process()
            {
                if (_queuedPulses.Count == 0) return false;
                var pulse = _queuedPulses.Dequeue();

                if (!_modules.TryGetValue(pulse.To, out var module))
                {
                    throw new InvalidDataException($"Pulse from [{pulse.From}] addressed to unknown device [{pulse.To}].");
                }

                module.OnPulse(pulse);
                return true;
            }

            internal static Device CreateDevice(string[] input)
            {
                var device = new Device();
                Dictionary<char, Type> deviceTypes =
                    Assembly.GetExecutingAssembly().GetTypes().Select(t => (type: t, prefix: t.GetCustomAttribute<PrefixAttribute>()))
                    .Where(t => t.prefix != null)
                    .ToDictionary(t => t.prefix.Prefix, t => t.type);

                //////// add special modules
                // add output module
                var output = new Output("", "");
                device._modules.Add(output.Name, output);
                // also, add rx module
                var rx = new Rx("", "");
                device._modules.Add(rx.Name, rx);
                // button
                var button = new Button("", BroadcastModuleName);
                device._modules.Add(button.Name, button);


                //////// add modules from schema
                foreach (var line in input)
                {
                    var sp1 = line.Split(DevSplitString, StringSplitOptions.TrimEntries);

                    var pr_name = sp1[0];
                    var prefix = pr_name[0];

                    if (!deviceTypes.TryGetValue(prefix, out var type))
                        throw new InvalidDataException($"Unable to create module {pr_name}: Module type not found.");

                    if (Activator.CreateInstance(type, new object[] { pr_name[1..], sp1[1] }) is not Module module)
                        throw new InvalidDataException($"Unable to create module {pr_name}: Unable to construct type.");
                    device._modules.Add(module.Name, module);
                }

                _device = device;

                // let modules process the schema, should they want to
                foreach (var module in device._modules.Values)
                    module.RaiseAfterMachineCreated(device);

                return device;
            }
        }

        #region Modules
        abstract class Module
        {
            public Module(string name, string targets)
            {
                _name = name;
                var sp = targets.Split(StdSplitChars, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                _targets = sp.ToList();
            }

            private readonly List<string> _targets = new();
            private readonly string _name;

            public virtual string Name => _name;
            public IReadOnlyList<string> Targets => _targets;

            public bool HasTarget(string name) => _targets.Contains(name);

            protected void SendPulseToAll(bool isHighPulse)
            {
                foreach (var target in _targets)
                    Device.TheDevice.SendPulse(new Day20.Pulse(Name, target, isHighPulse));
            }

            internal abstract void OnPulse(Pulse pulse);
            internal void RaiseAfterMachineCreated(Device device) => OnAfterMachineCreated(device);
            protected abstract void OnAfterMachineCreated(Device device);
            protected void ConnectToModule(string otherModule)
            {
                if (Device.TheDevice.HasModule(otherModule) == false)
                    throw new InvalidDataException($"Unable to connect to module {otherModule}: Module not found.");
                _targets.Add(otherModule);
            }
        }

        /// <summary>
        /// This module is a special module, for testing only.
        /// It is being added automatically.
        /// </summary>
        [Prefix((char)0)]
        class Output : Module
        {
            public override string Name => OutputModuleName;
            public Output(string name, string targets) : base(name, targets)
            {
            }

            internal override void OnPulse(Pulse pulse)
            {
            }

            protected override void OnAfterMachineCreated(Device device)
            {
            }
        }

        /// <summary>
        /// Flip-flop modules (prefix %) are either on or off; they are initially off. 
        /// If a flip-flop module receives a high pulse, it is ignored and nothing happens. 
        /// However, if a flip-flop module receives a low pulse, it flips between on and off. 
        /// If it was off, it turns on and sends a high pulse. 
        /// If it was on, it turns off and sends a low pulse.
        /// </summary>
        [Prefix('%')]
        class FlipFlop : Module
        {
            public FlipFlop(string name, string targets) : base(name, targets) { }

            private bool _state = false; // start on off state
            public bool State => _state;

            internal override void OnPulse(Pulse pulse)
            {
                if (pulse.IsHighPulse) return;
                _state = !_state;
                SendPulseToAll(_state);
            }

            protected override void OnAfterMachineCreated(Device device) { }
        }

        /// <summary>
        /// Conjunction modules (prefix &) remember the type of the most recent pulse 
        /// received from each of their connected input modules; 
        /// they initially default to remembering a low pulse for each input. 
        /// When a pulse is received, the conjunction module first updates its memory 
        /// for that input. Then, if it remembers high pulses for all inputs, 
        /// it sends a low pulse; otherwise, it sends a high pulse.
        /// </summary>
        [Prefix('&')]
        class Conjunction : Module
        {
            public Conjunction(string name, string targets) : base(name, targets) { }

            public Dictionary<string, bool> memory = new();

            protected override void OnAfterMachineCreated(Device device)
            {
                // see which devices have our device as output
                // remember them all
                var all = device.GetAllModules();
                foreach (var module in all)
                {
                    if (module.HasTarget(Name))
                        memory.Add(module.Name, false);
                }
            }

            internal override void OnPulse(Pulse pulse)
            {
                if (!memory.TryGetValue(pulse.From, out _))
                    throw new InvalidDataException($"Conjunction module {Name} does not have a memory slot for input {pulse.From}");

                memory[pulse.From] = pulse.IsHighPulse;

                if (memory.Any(m => m.Value == false))
                    SendPulseToAll(HIGH); // note: low -> high. See description
                else
                    SendPulseToAll(LOW); // note: high -> low. See description
            }
        }

        /// <summary>
        /// There is a single broadcast module (named broadcaster). 
        /// When it receives a pulse, it sends the same pulse to all of its destination modules.
        /// </summary>
        [Prefix('b')]
        class Broadcaster : Module
        {
            public override string Name => BroadcastModuleName;
            public Broadcaster(string name, string targets) : base(name, targets) { }

            internal override void OnPulse(Pulse pulse) => SendPulseToAll(pulse.IsHighPulse);

            protected override void OnAfterMachineCreated(Device device)
            {
            }
        }


        /// <summary>
        /// Here at Desert Machine Headquarters, 
        /// there is a module with a single button on it called, aptly, the button module. 
        /// When you push the button, a single low pulse is sent directly to the broadcaster module.
        /// </summary>
        [Prefix((char)1)]
        class Button : Module
        {
            public override string Name => ButtonModuleName;
            public Button(string name, string targets) : base(name, targets) { }

            public void Push()
            {
                SendPulseToAll(LOW);
            }
            internal override void OnPulse(Pulse pulse)
            {
            }

            protected override void OnAfterMachineCreated(Device device)
            {
            }
        }

        /// <summary>
        /// The final machine responsible for moving the sand down to Island Island has a module attached named rx. 
        /// The machine turns on when a single low pulse is sent to rx.
        /// Reset all modules to their default states.
        /// Waiting for all pulses to be fully handled after each button press, 
        /// what is the fewest number of button presses required to deliver a single 
        /// low pulse to the module named rx?
        /// </summary>
        class Rx : Module
        {
            public override string Name => RxModuleName;
            public Rx(string name, string targets) : base(name, targets) { }
            private bool _state = true; // start in on state
            public bool State => _state;

            public bool IsLow => _state == false;
            internal override void OnPulse(Pulse pulse) { _state = pulse.IsHighPulse; }

            protected override void OnAfterMachineCreated(Device device) { }
        }
        #endregion


        private static void PushAndProcess(Device device, Button button)
        {
            button.Push();
            while (device.Process())
                Thread.Sleep(0);  
        }
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string[] input)
        {
            var device = Device.CreateDevice(input);
            // push the button
            var button = (device.GetModule(ButtonModuleName) as Button);

            for (int i = 0; i < 1000; i++)
            {
                PushAndProcess(device, button);
            }
            return device.LowLevelPulsesCount * device.HighLevelPulsesCount;
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