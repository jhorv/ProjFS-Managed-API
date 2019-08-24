namespace SimpleProviderManaged

open System
open System.IO

type ProjectedFileInfo
    ( name : string,
      size : uint64,
      isDirectory : bool,
      creationTime : DateTime,
      lastAccessTime : DateTime,
      lastWriteTime : DateTime,
      changeTime : DateTime,
      attributes : FileAttributes
    ) =
    
    // Make sure the directory attribute is stored properly.
    let attributes =
        if isDirectory then
            attributes ||| FileAttributes.Directory
        else
            attributes &&& ~~~FileAttributes.Directory

    new (name : string, size : uint64, isDirectory : bool) =
        let now = DateTime.UtcNow
        let attributes = if isDirectory then FileAttributes.Directory else FileAttributes.Normal
        ProjectedFileInfo(name, size, isDirectory, now, now, now, now, attributes)

    member this.Name with get() = name
    member this.Size with get() = size
    member this.IsDirectory with get() = isDirectory
    member this.CreationTime with get() = creationTime
    member this.LastAccessTime with get() = lastAccessTime
    member this.LastWriteTime with get() = lastWriteTime
    member this.ChangeTime with get() = changeTime
    member this.Attributes with get() = attributes
