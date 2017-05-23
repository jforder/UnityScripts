using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class ThetaCameraManager : MonoBehaviour
{
   
    private string sessionID;

    // Use this for initialization
    void Start()
    {
        //StartSession();
        //TakePicture();

        string[] opts = new[] {"1", "2"};
        GetOptions(opts);

    }

    void SendRequest(PostRequestsNonParma request)
    {
        string post = JsonUtility.ToJson(request, true);
        HTTPHandler.PostRequest(post);
    }

    void SendRequest(PostRequestsList request)
    {
        string post = JsonUtility.ToJson(request, true);
        HTTPHandler.PostRequest(post);
    }

    void SendRequest(PostRequestsOptions request)
    {
        string post = JsonUtility.ToJson(request, true);
        HTTPHandler.PostRequest(post);
    }

    void StartSession()
    {
        PostRequestsNonParma request = new PostRequestsNonParma("startSession", new PostParamsBase());
        SendRequest(request);

        //Post to http client
    }

    void DisconnectFromLAN()
    {
        PostRequestsNonParma request = new PostRequestsNonParma("_finishWlan", new PostParamsBase());
        SendRequest(request);
    }

    void TakePicture()
    {
        PostRequestsNonParma request = new PostRequestsNonParma("takePicture", new PostParamsBase());
        SendRequest(request);
    }

    void StartCapture(string mode)
    {
        PostRequestsNonParma request = new PostRequestsNonParma("startCapture", new PostPramasStartCapture(mode));
        SendRequest(request);
    }

    void StopCapture(string mode)
    {
        PostRequestsNonParma request = new PostRequestsNonParma("stopCapture", new PostParamsBase());
        SendRequest(request);
    }

    void GetFileList(string fileType, int startPosition, int entryCount, int maxThumbSize, bool detail, string sort)
    {
        PostRequestsList request = new PostRequestsList("listFiles", new PostParamsListFiles(fileType, startPosition, entryCount, maxThumbSize, detail, sort));
        SendRequest(request);
    }

    void Delete(string[] fileUrls)
    {
        PostRequestsOptions request = new PostRequestsOptions("delete", new PostParamsOptions(fileUrls));
        SendRequest(request);
    }

    void Reset()
    {
        PostRequestsNonParma request = new PostRequestsNonParma("reset", new PostParamsBase());
        SendRequest(request);
    }

    void GetOptions(string[] optionNames)
    {
        PostRequestsOptions request = new PostRequestsOptions("getOptions", new PostParamsOptions(optionNames));
        SendRequest(request);
    }

    //Break these down into more detials methods - e.g. SetISO, SetApature
    //void SetOptions(PostSetOptions options)
    //{
    //    PostRequests postrequest = new PostRequests("getOptions", new PostParamsSetOptions(optionNames));
    //    string postToJson = JsonUtility.ToJson(postrequest);
    //}


    //string JsonCreator(string commandName, string[] commandParams)
    //{
    //    string template = "{" +
    //                       "name:"  " #COMMANDNAME# " +
    //                      "}"

    //    return "";
    //}

    //string JsonCreator(string commandName, string[] commandParams, string[] options)
    //{
    //    return "";
    //}

}
	