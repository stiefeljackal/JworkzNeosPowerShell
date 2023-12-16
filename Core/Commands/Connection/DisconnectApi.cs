﻿using System.Management.Automation;

namespace Jworkz.ResonitePowerShellModule.Core.Commands.Connection;

using Abstract;
using Clients.Abstract;
using Utilities;

/// <summary>
/// Disconnects from the Resonite Api interface via CloudX with either the default or provided client
/// </summary>
[Cmdlet("Disconnect", "ResoniteApi")]
[OutputType(typeof(void))]
public class DisconnectApi : BasePSCmdlet
{
    /// <summary>
    /// Optional client to send the request to intead of the current default one
    /// </summary>
    [Parameter]
    public ISkyFrostInterfaceClient? Client;

    protected override void ProcessRecord()
    {
        if (!IsParamSpecified("Client"))
        {
            Client = SkyFrostInterfacePool.Current ?? throw new InvalidOperationException(ErrorActionSpecified);
        }
        else if (Client == null)
        {
            throw new InvalidOperationException("Specified client is null.");
        }

        Client?.Logout();

        if (Client == SkyFrostInterfacePool.Current)
        {
            SkyFrostInterfacePool.Current = null;
        }
    }
}
