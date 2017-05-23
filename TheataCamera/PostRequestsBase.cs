using UnityEngine;
using System;
using System.Collections;

#region Execute Methods

[Serializable]
public class PostRequestsNonParma
{
    [SerializeField]
    private string name;
    [SerializeField]
    private PostParamsBase parameters;

    public PostRequestsNonParma(string postName, PostParamsBase postParas)
    {
        name = postName;
        parameters = postParas;
    }

}

[Serializable]
public class PostRequestsList
{
    [SerializeField]
    private string name;
    [SerializeField]
    private PostParamsListFiles parameters;

    public PostRequestsList(string postName, PostParamsListFiles postParas)
    {
        name = postName;
        parameters = postParas;
    }

}

[Serializable]
public class PostRequestsOptions
{
    [SerializeField]
    private string name;
    [SerializeField]
    private PostParamsOptions parameters;

    public PostRequestsOptions(string postName, PostParamsOptions postParas)
    {
        name = postName;
        parameters = postParas;
    }
}

#endregion

#region Params

//_mode : string
//fileType : sting
//startPosition: int
//entryCount : int
//maxThumbSize : int
//_detail : bool
//_sort : string
//fileUrls : string array
//optionNames : string array
//options : Object

[Serializable]
public class PostParamsBase
{}

[Serializable]
public class PostEmptyPramas {}

[Serializable]
public class PostPramasStartCapture : PostParamsBase
{
    [SerializeField]
    private string _mode;

    public PostPramasStartCapture(string mode)
    {
        _mode = mode;
    }
}

[Serializable]
public class PostParamsListFiles : PostParamsBase
{
    [SerializeField] private string fileType;
    [SerializeField] private int startPostion;
    [SerializeField] private int entryCount;
    [SerializeField] private int maxThumbSize;
    [SerializeField] private bool _detail;
    [SerializeField] private string _sort;

    public PostParamsListFiles(string FileType, int StartPos, int EntryCount, int MaxThumbSize, bool Detail, string Sort)
    {
        fileType = FileType;
        startPostion = StartPos;
        entryCount = EntryCount;
        maxThumbSize = MaxThumbSize;
        _detail = Detail;
        _sort = Sort;
    }
}

[Serializable]
public class PostPramasDeleteFiles : PostParamsBase
{
    [SerializeField] private string[] fileUrls;

    public PostPramasDeleteFiles(string[] filePaths)
    {
        fileUrls = filePaths;
    }
}

[Serializable]
public class PostParamsOptions : PostParamsBase
{
    [SerializeField]
    public string[] optionNames;

    public PostParamsOptions(string[] options)
    {
        optionNames = options;
    }
}

#endregion

#region Options

[Serializable]
public class SetOptions
{
    
}

#endregion
