using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

var lines = File.ReadAllLines(@"C:\Users\wikto\Downloads\Erosion\Erosion.log");
List<string> outputLines = [];

Dictionary<string, int> toRemoveSingleLine = new()
{
    { "____", 0 },
    { "Mesh is marked for CPU read", 0 },
    { "r.Lumen", 0 },
    { "Mounting Engine plugin", 0 },
    { "LogWorldPartition: Data Layer Instance", 0 },
    { "LogScript: Warning: Script Msg: Divide by zero: Divide_IntInt", 5 },
    { "LogScript: Warning: Accessed None trying to read property CallFunc_CreateDynamicMaterialInstance_ReturnValue_2", 9 },
    {  "Material index 0 is invalid.", 7 },
    { "Warning: Accessed None trying to read property ActionEnterMaterialInstanc", 6 },

    { "LogScript: Warning: Script call stack:", 0 },
    { "/ErosionCore/Vehicles/BP_ErosionVehicle_Base.BP_ErosionVehicle_Base_C:Timeline__UpdateFunc", 0 },
    { "/ErosionCore/Vehicles/BP_ErosionVehicle_Base.BP_ErosionVehicle_Base_C:ExecuteUbergraph_BP_ErosionVehicle_Base", 0 },
    { "PIE: Warning: AttachTo: '/ErosionUpperWorld/Maps/L_UpperWorld_Main/_Generated_", 0 },
    { "Warning: Ignoring primary asset ", 0 },
    { "Spawner: ", 0 },
    { "Targetting: ", 0 },
     

};

bool check(string line, out int lto)
{
    foreach (var pair in toRemoveSingleLine)
    {
        if (line.Contains(pair.Key))
        {
            lto = pair.Value;
            return true;
        }
    }
    lto = 0;
    return false;
}
for (var i = 0; i < lines.Length; i++)
{
    var line = lines[i];
    if (check(line, out var lto))
    {
        i += lto;
        continue;
    }

    outputLines.Add(line);
}

File.WriteAllLines(@"C:\Users\wikto\Downloads\Erosion\Erosion2.log", outputLines.ToArray());