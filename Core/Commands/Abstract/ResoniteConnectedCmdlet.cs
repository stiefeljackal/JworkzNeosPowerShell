﻿using System.Management.Automation;

namespace Jworkz.ResonitePowerShellModule.Core.Commands.Abstract;

using Clients.Abstract;
using Utilities;

/// <summary>
/// Base class for cmdlets that require a Resonite connection
/// </summary>
public class ResoniteConnectedCmdlet : BasePSCmdlet
{
    /// <summary>
    /// Client to use for Resonite Api calls
    /// </summary>
    [Parameter(HelpMessage = "Optional client to be used. Defaults to the default current client if null.")]
    public ISkyFrostInterfaceClient? Client;

    protected override void BeginProcessing()
    {
        base.BeginProcessing();

        if (Client == null)
        {
            Client = SkyFrostInterfacePool.Current ?? throw new InvalidOperationException("A client has not been established yet. Use Connect-ResoniteConnectApi to connect or provide a valid client using -Client.");
        }
    }

    protected override void ProcessRecord()
    {
        try
        {
            ExecuteCmdlet();
        }
        catch (PipelineStoppedException) { throw; }
        catch (Exception ex)
        {
            var errorMessage = ex.Message;

            if (!HasStoppingErrorAction())
            {
                throw new PSInvalidOperationException(errorMessage);
            }

            if (!HasIgnoreErrorAction())
            {
                ex.Data["TimeStampUtc"] = DateTime.UtcNow;

                var errDetails = new ErrorDetails(errorMessage);
                var errRecord = new ErrorRecord(ex, "EXCEPTION", ErrorCategory.WriteError, null);
                errRecord.ErrorDetails = errDetails;

                WriteError(errRecord);
            }
        }
    }
}