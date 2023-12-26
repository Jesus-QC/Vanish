using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using NorthwoodLib.Pools;
using RemoteAdmin.Communication;

namespace Vanish.Patches;

[HarmonyPatch(typeof(RaPlayerList), nameof(RaPlayerList.ReceiveData), [typeof(CommandSender), typeof(string)])]
public class RaPlayerListPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Beq) + 1;

        Label skip = generator.DefineLabel();
        newInstructions[index].labels.Add(skip);
        
        newInstructions.InsertRange(index, new CodeInstruction[]
        {
            // If is global mod we dont hide the vanished player
            new (OpCodes.Ldarg_1),
            new (OpCodes.Call, AccessTools.Method(typeof(RaPlayerListPatch), nameof(IsGlobalMod))),
            new (OpCodes.Brtrue_S, skip),
            
            // We don't show the player if it isn't vanished
            new (OpCodes.Ldloc_S, 9),
            new (OpCodes.Call, AccessTools.Method(typeof(EntryPoint), nameof(EntryPoint.IsVanished))),
            new (OpCodes.Brtrue, newInstructions[index - 1].operand),
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }

    private static bool IsGlobalMod(CommandSender sender)
    {
        foreach (ReferenceHub player in ReferenceHub.AllHubs)
        {
            if (player.authManager.UserId != sender.SenderId)
                continue;

            return player.authManager.RemoteAdminGlobalAccess;
        }

        return false;
    }
}