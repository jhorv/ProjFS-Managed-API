namespace SimpleProviderManaged

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.IO
open System.Linq
open System.Runtime.InteropServices
open Microsoft.Windows.ProjFS
open Serilog

type SimpleProvider (options : ProviderOptions) =
    member this.ReproduceError() =
        let hr = HResult.AccessDenied
        if hr = HResult.AlreadyInitialized then
            Log.Information("Already initialized")
        ()

(*
[<AllowNullLiteral>]
type NotificationCallbacks
    (
    provider : SimpleProvider,
    virtInstance : VirtualizationInstance,
    notificationMappings : IReadOnlyCollection<NotificationMapping>) =
    
    do
        // Look through notificationMappings for all the set notification bits.  Supply a callback
        // for each set bit.
        let mutable notification = NotificationType.None
        for mapping in notificationMappings do
            notification <- notification ||| mapping.NotificationMask
    
        if ((notification &&& NotificationType.FileOpened) = NotificationType.FileOpened) then
            virtInstance.OnNotifyFileOpened <- new NotifyFileOpenedCallback(this.NotifyFileOpenedCallback)

    member this.NotifyFileOpenedCallback
        (
            relativePath : string,
            isDirectory : bool,
            triggeringProcessId : uint32,
            triggeringProcessImageFileName : string,
            [<Out>] notificationMask : NotificationType byref
        ) =
        Log.Information("NotifyFileOpenedCallback [{relativePath}]", relativePath)
        Log.Information("  Notification triggered by [{triggeringProcessImageFileName} {triggeringProcessId}]", triggeringProcessImageFileName, triggeringProcessId)
        notificationMask <- NotificationType.UseExistingMask
        provider.SignalIfTestMode("FileOpened") |> ignore
        true


and SimpleProvider (options : ProviderOptions) =
    let scratchRoot = options.VirtRoot
    let layerRoot = options.SourceRoot
    let activeEnumerations = new ConcurrentDictionary<Guid, ActiveEnumeration>()
    let mutable virtualizationInstance : VirtualizationInstance = null
    let mutable notificationCallbacks : NotificationCallbacks = null

    //do
    //    if options.TestMode then
    //        options.EnableNotifications <- true

    //    let notificationMappings =
    //        if options.EnableNotifications then
    //            let rootName = ""
    //            seq {
    //                yield new NotificationMapping(
    //                    NotificationType.FileOpened
    //                    ||| NotificationType.NewFileCreated
    //                    ||| NotificationType.FileOverwritten
    //                    ||| NotificationType.PreDelete
    //                    ||| NotificationType.PreRename
    //                    ||| NotificationType.PreCreateHardlink
    //                    ||| NotificationType.FileRenamed
    //                    ||| NotificationType.HardlinkCreated
    //                    ||| NotificationType.FileHandleClosedNoModification
    //                    ||| NotificationType.FileHandleClosedFileModified
    //                    ||| NotificationType.FileHandleClosedFileDeleted
    //                    ||| NotificationType.FilePreConvertToFull,
    //                    rootName)
    //            }
    //            |> List<NotificationMapping>
    //        else
    //            List<NotificationMapping>()

    //    try
    //        virtualizationInstance <-
    //            new VirtualizationInstance(
    //                scratchRoot,
    //                poolThreadCount = 0,
    //                concurrentThreadCount = 0,
    //                enableNegativePathCache = false,
    //                notificationMappings: notificationMappings
    //            )
    //    with
    //        | ex ->
    //            Log.Fatal(ex, "Failed to create VirtualizationInstance.")
    //            reraise()

    //    notificationCallbacks <-
    //        NotificationCallbacks(this, virtualizationInstance, notificationMappings)

    //    Log.Information("Created instance. Layer [{Layer}], Scratch [{Scratch}]", this.layerRoot, this.scratchRoot)

    //    if options.TestMode then
    //        Log.Information("Provider started in TEST MODE.")

    member this.StartVirtualization () : bool =
        // Optional callbacks
        virtualizationInstance.OnQueryFileName <- new QueryFileNameCallback(this.QueryFileNameCallback)

        let hr = virtualizationInstance.StartVirtualizing(this :> IRequiredCallbacks)
        if hr <> HResult.Ok then
            Log.Error("Failed to start virtualization instance: {Result}", hr)
            false

        // If we're running in test mode, signal the test that it may proceed.  If this fails
        // it means we had some problem accessing the shared event that the test set up, so we'll
        // stop the provider.
        elif not (this.SignalIfTestMode("ProviderTestProceed")) then
            virtualizationInstance.StopVirtualizing()
            false
        else
            true

    member this.SignalIfTestMode (eventName : string) : bool =
        raise (NotImplementedException())

    member private this.QueryFileNameCallback (relativePath : string) : HResult =
        Log.Information("----> QueryFileNameCallback relativePath [{Path}]", relativePath)

        let parentDirectory = Path.GetDirectoryName(relativePath)
        let childName = Path.GetFileName(relativePath)
        let hr =
            if (this.GetChildItemsInLayer(parentDirectory).Any(fun child -> Utils.IsFileNameMatch(child.Name, childName))) then
                HResult.Ok;
            else
                HResult.FileNotFound
        Log.Information("<---- QueryFileNameCallback {Result}", hr)
        hr

    member private this.GetChildItemsInLayer (relativePath : string) : ProjectedFileInfo seq =
        raise (NotImplementedException())

    interface IRequiredCallbacks with
        member this.StartDirectoryEnumerationCallback
            (
            commandId : int,
            enumerationId : Guid,
            relativePath : string,
            triggeringProcessId : uint32,
            triggeringProcessImageFileName : string
            ) : HResult =
            NotImplementedException() |> raise

        member this.GetDirectoryEnumerationCallback
            (
            commandId : int,
            enumerationId : Guid,
            filterFIleName : string,
            restartScan : bool,
            enumResult : IDirectoryEnumerationResults
            ) : HResult =
            NotImplementedException() |> raise
        
        member this.EndDirectoryEnumerationCallback
            (
            enumerationId : Guid
            ) : HResult =
            NotImplementedException() |> raise

        member this.GetPlaceholderInfoCallback
            (
            commandId : int,
            relativePath : string,
            triggeringProcessId : uint32,
            triggeringProcessImageFileName : string
            ) : HResult =
            NotImplementedException() |> raise

        member this.GetFileDataCallback
            (
            commandId : int,
            relativePath : string,
            byteOffset : uint64,
            length : uint32,
            dataStreamId : Guid,
            contentId : byte[],
            providerId : byte[],
            triggeringProcessId : uint32,
            triggeringProcessImageFileName : string
            ) : HResult =
            NotImplementedException() |> raise
*)