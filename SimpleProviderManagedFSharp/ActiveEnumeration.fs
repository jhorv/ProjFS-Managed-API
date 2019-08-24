namespace SimpleProviderManaged

open System
open System.Collections.Generic

type ActiveEnumeration (fileInfos : List<ProjectedFileInfo>) =
    let mutable fileInfoEnumerator : IEnumerator<ProjectedFileInfo> = null
    let mutable filterString : string = null
    let mutable isCurrentValid = false

    let isCurrentHidden () =
        not (ActiveEnumeration.FileNameMatchesFilter(fileInfoEnumerator.Current.Name, filterString))

    let moveNext () =
        isCurrentValid <- fileInfoEnumerator.MoveNext()
        while isCurrentValid && isCurrentHidden() do
            isCurrentValid <- fileInfoEnumerator.MoveNext()
        isCurrentValid

    let resetEnumerator () =
        fileInfoEnumerator <- fileInfos.GetEnumerator()

    let saveFilter (filter : string) =
        if String.IsNullOrEmpty(filter) then
            filterString <- String.Empty
        else
            filterString <- filter
            if isCurrentValid && isCurrentHidden() then
                moveNext() |> ignore

    do
        resetEnumerator()
        moveNext() |> ignore

    /// true if Current refers to an element in the enumeration, false if Current is past the end of the collection
    member this.IsCurrentValid with get () = isCurrentValid

    /// Gets the element in the collection at the current position of the enumerator
    member this.Current with get () = fileInfoEnumerator.Current

    /// Resets the enumerator and advances it to the first ProjectedFileInfo in the enumeration
    /// <param name="filter">Filter string to save.  Can be null.</param>
    member this.RestartEnumeration (filter : string) =
        resetEnumerator()
        isCurrentValid <- fileInfoEnumerator.MoveNext()
        saveFilter filter

    /// Advances the enumerator to the next element of the collection (that is being projected).   
    /// If a filter string is set, MoveNext will advance to the next entry that matches the filter.
    /// <returns>
    /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection
    /// </returns>
    member this.MoveNext () = moveNext()

    /// <summary>
    /// Attempts to save the filter string for this enumeration.  When setting a filter string, if Current is valid
    /// and does not match the specified filter, the enumerator will be advanced until an element is found that
    /// matches the filter (or the end of the collection is reached).
    /// </summary>
    /// <param name="filter">Filter string to save.  Can be null.</param>
    /// <returns> True if the filter string was saved.  False if the filter string was not saved (because a filter string
    /// was previously saved).
    /// </returns>
    /// <remarks>
    /// Per MSDN (https://msdn.microsoft.com/en-us/library/windows/hardware/ff567047(v=vs.85).aspx, the filter string
    /// specified in the first call to ZwQueryDirectoryFile will be used for all subsequent calls for the handle (and
    /// the string specified in subsequent calls should be ignored)
    /// </remarks>
    member this.TrySaveFilterString (filter : string) =
        if isNull filterString then
            saveFilter filter
            true
        else
            false

    /// <summary>
    /// Returns the current filter string or null if no filter string has been saved
    /// </summary>
    /// <returns>The current filter string or null if no filter string has been saved</returns>
    member this.GetFilterString() = filterString

    static member FileNameMatchesFilter (name : string, filter : string) =
        if String.IsNullOrEmpty(filter) then
            true
        elif filter = "*" then
            true
        else
            Microsoft.Windows.ProjFS.Utils.IsFileNameMatch(name, filter)
