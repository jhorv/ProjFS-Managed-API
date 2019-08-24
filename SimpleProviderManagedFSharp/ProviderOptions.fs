namespace SimpleProviderManaged
open CommandLine

type ProviderOptions () =
    [<Option(Required = true, HelpText = "Path to the virtualization root.")>]
    member val VirtRoot : string = null with get, set
    
    [<Option(Required = true, HelpText = "Path to the files and directories to project.")>]   
    member val SourceRoot : string = null with get, set
    
    [<Option('t', "testmode", HelpText = "Use this when running the provider with the test package.", Hidden = true)>]
    member val TestMode = false with get, set
    
    [<Option('n', "notifications", HelpText = "Enable file system operation notifications.")>]
    member val EnableNotifications = false with get, set
    
    [<Option('d', "denyDeletes", HelpText = "Deny deletes.", Hidden = true)>]
    member val DenyDeletes = false with get, set
    
    [<Text.Usage(ApplicationAlias = "SimpleProviderManaged")>]
    static member Examples
        with get () = seq {
            yield Text.Example(
                "Start provider, projecting files and directories from 'c:\\source' into 'c:\\virtRoot'",
                ProviderOptions(SourceRoot = "c:\\source", VirtRoot = "c:\\virtRoot"))
        }
