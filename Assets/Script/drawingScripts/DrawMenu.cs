using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class DrawMenu : MonoBehaviour
{
    public Camera cam;

    //these don't have to do with the UI : 
    [SerializeField]
    public List<string> GareNames;
    private GareDrawing lastGare;
    [SerializeField]
    public Material GareTexture;
    public DrawingBoard board;
    private List<int> IndicesLeft;//to avoid giving the same name twice.
    public bool WARNING_ENABLED;
    public float WARNING_TIME;
    private bool POPUP_TOGGLED;
    public List<string> temp_names;//names within one popup menu instance.

    //these do :
    public TextMeshPro info;
    public TextMeshPro warning;
    public GameObject PopupBackground;
    public Button option1;
    public Button option2;
    public Button option3;


    private void Start()
    {
        Controls.FORCE_RAYCAST = true;
        POPUP_TOGGLED=true;//next line inverts this.
        TogglePopup();//un-show the popup at the start
        warning.gameObject.SetActive(false);//disable the warning separately
        temp_names = new List<string>();
        IndicesLeft = new List<int>();
        CheckTakenIndices();//fill IndicesLeft with indices.
    }

    private void Update()
    {
        CheckWarning();
    }

    public void CreateButtonRouter(){
        if(POPUP_TOGGLED){
            TogglePopup();//disable popup (cancel creation)
            GameObject.Find("Main/makeGare").GetComponent<TextChangeScript>().SetText("Créer une gare");
        }else{
            if(StartGareCreation()){//if the pencil was on the paper
                GameObject.Find("Main/makeGare").GetComponent<TextChangeScript>().SetText("Annuler");
            }
        }
    }

    bool StartGareCreation(){
        if(board.pencilIsDrawing()){
            TogglePopup();//show menu
            temp_names.Clear();
            CheckTakenIndices();
            while(temp_names.Count<3){
                int idx=IndicesLeft[Random.Range(0,IndicesLeft.Count)];
                if(IndicesLeft.Contains(idx)){
                    IndicesLeft.Remove(idx);
                    temp_names.Add(GareNames[idx]);
                }
            }
            putGareNames(temp_names);//assign them to the buttons
            return true;
        }else{
            warning.gameObject.SetActive(true);//toggle warning, which will last 2 seconds.
            WARNING_ENABLED=true;
            return false;
        }
    }

    void MakeFirstGare(){
        Debug.Log("called MakeFirstGare");
    }

    void TogglePopup(){
        POPUP_TOGGLED = !POPUP_TOGGLED;
        info.gameObject.SetActive(POPUP_TOGGLED);
        PopupBackground.gameObject.SetActive(POPUP_TOGGLED);
        option1.gameObject.SetActive(POPUP_TOGGLED);
        option2.gameObject.SetActive(POPUP_TOGGLED);
        option3.gameObject.SetActive(POPUP_TOGGLED);
    }

    public void CreateGare(int nameChoice){
        if(lastGare is null){MakeFirstGare();}
        Vector3 garePos = board.PencilPosition();
        GameObject gare = (GameObject)(Instantiate(Resources.Load("Prefabs/text/GareText")));
        gare.GetComponent<GareDrawing>().Init(temp_names[nameChoice]);
        gare.transform.SetParent(GameObject.Find("/Plane").transform);
        gare.GetComponent<RectTransform>().localPosition=garePos;
        //gare.GetComponent<RectTransform>().localRotation=new Quaternion(0,1,0,90);
    }

    public void save(){
        Debug.Log("save not yet implemented ! ");
    }

    void CheckTakenIndices(){
        if(IndicesLeft.Count<=3){
            IndicesLeft.Clear();
            for(int i=0;i<GareNames.Count;i++){IndicesLeft.Add(i);}
        }
    }

    void putGareNames(List<string> names){
        option1.GetComponent<TextChangeScript>().SetText(names[0]);
        option2.GetComponent<TextChangeScript>().SetText(names[1]);
        option3.GetComponent<TextChangeScript>().SetText(names[2]);
    }

    void CheckWarning(){
        if(WARNING_ENABLED){
            WARNING_TIME+=Time.deltaTime;
            if(WARNING_TIME>2){
                WARNING_TIME=0;
                warning.gameObject.SetActive(false);
                WARNING_ENABLED = false;
            }
        }
    }
}