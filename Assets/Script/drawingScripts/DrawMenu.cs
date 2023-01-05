using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Splines;
using TMPro;
using Unity.VisualScripting;

public class DrawMenu : MonoBehaviour
{
    public Camera cam;

    //these don't have to do with the UI : 
    [SerializeField]
    public List<string> GareNames;
    private GameObject lastGare;
    public DrawingBoard board;
    private List<int> IndicesLeft;//to avoid giving the same name twice.
    private bool WARNING_ENABLED;
    private float WARNING_TIME;
    private bool POPUP_TOGGLED;
    private List<string> temp_names;//names within one popup menu instance.
    private Gare oldSaveGare;

    //pour la sauvegarde :
    public List<Gare> gares = new List<Gare>();
    public List<Ligne> lignes = new List<Ligne>();



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
            temp_names.Clear();
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


    void TogglePopup(){
        POPUP_TOGGLED = !POPUP_TOGGLED;
        info.gameObject.SetActive(POPUP_TOGGLED);
        PopupBackground.gameObject.SetActive(POPUP_TOGGLED);
        option1.gameObject.SetActive(POPUP_TOGGLED);
        option2.gameObject.SetActive(POPUP_TOGGLED);
        option3.gameObject.SetActive(POPUP_TOGGLED);
    }

    public void CreateGare(int nameChoice){
        if(lastGare is null && board.sContainer.Spline.Count>1){//only if the rail has moved
            CreateFirstGare();}
        GameObject gare = (GameObject)(Instantiate(Resources.Load("Prefabs/text/GareText")));
        Vector3 garePos = board.LocalPencilPosition();

        splineMesh spmesh = board.sContainer.GetComponent<splineMesh>();
        gare.GetComponent<GareDrawing>().init(GameObject.Find("/Plane").transform, garePos,temp_names[nameChoice],
                                                        spmesh.lastNormal);
        //create an optimizer class later
        if(lastGare is not null){
            OptimizeSpline();
            createSaveableObjects(gare);//create a line and all
            CreateNewSplineMesh();
        }else{//just set this gare. This only happens if the user creates one before drawing.
            oldSaveGare = new Gare(gare.GetComponent<GareDrawing>().strText,
            gare.transform.localPosition, new List<Ligne>());
            gares.Add(oldSaveGare);
        }
        lastGare = gare;
        CreateButtonRouter();//close the popup
        spmesh.ClearSpline();//start the new line
    }
    void CreateFirstGare(){
        GameObject gare = (GameObject)(Instantiate(Resources.Load("Prefabs/text/GareText")));
        Vector3 garePos = new Vector3(board.sContainer.Spline.EvaluatePosition(0)[0],0.0f,
                                      -board.sContainer.Spline.EvaluatePosition(0)[2]);
        gare.GetComponent<GareDrawing>().init(GameObject.Find("/Plane").transform,garePos,
                                                "Oubliettes",new Vector3(0,0,0));

        oldSaveGare = new Gare(gare.GetComponent<GareDrawing>().strText,
            gare.transform.localPosition,
            new List<Ligne>());
        lastGare = gare;
        //for save
        oldSaveGare = new Gare(gare.GetComponent<GareDrawing>().strText,
            gare.transform.localPosition,
            new List<Ligne>());
        gares.Add(oldSaveGare);
    }

    private void OptimizeSpline(){
        Debug.Log("optimizing path of length "+board.sContainer.Spline.Count.ToString());
    }

    public void save(){
        Debug.Log("number of gares : "+gares.Count.ToString());
        Debug.Log("number of lignes : "+lignes.Count.ToString());
        for(int i=0;i<gares.Count;i++){
            Debug.Log("gare"+i.ToString()+" has "+gares[i].liaisons.Count.ToString());
        }
        SaveLoad.GetInstance().Save(new DataTrainPath(gares, lignes), "TrainPath/one.data");
    }

    void CheckTakenIndices(){
        if(IndicesLeft.Count<=3){
            IndicesLeft.Clear();
            for(int i=0;i<GareNames.Count;i++){IndicesLeft.Add(i);}
        }
    }

    private void CreateNewSplineMesh(){
        GameObject spmesh = (GameObject)(Instantiate(Resources.Load("Prefabs/SplineMesh/Spline")));
        spmesh.GetComponent<splineMesh>().init(
            GameObject.Find("/Plane").transform,board.sContainer.GetComponent<splineMesh>());
        Spline newsp = new Spline();//into which to copy the former line's spline for display
        newsp.Copy(board.sContainer.GetComponent<splineMesh>().spline.Spline);
        spmesh.GetComponent<splineMesh>().spline.Spline = newsp;
        spmesh.GetComponent<splineMesh>().spline.Spline.EditType=SplineType.CatmullRom;
    }

    private void createSaveableObjects(GameObject gare){
        //create new link
        Gare newSaveGare = new Gare(gare.GetComponent<GareDrawing>().strText,
            gare.transform.localPosition,
            new List<Ligne>());
        Lien newlk = new Lien(newSaveGare,1);
        Lien oldlk = new Lien(oldSaveGare,0);
        //assign new link from last and new gares.
        Ligne newln = new Ligne(oldlk, newlk,
                                board.sContainer.Spline.CloneViaSerialization());
        Debug.Log("created link between "+oldSaveGare.cityName+" and "+
                                          newSaveGare.cityName);
        //assign new line to last gare and new gare.
        newSaveGare.liaisons.Add(newln);
        oldSaveGare.liaisons.Add(newln);
        oldSaveGare = newSaveGare;

        //add the new gare and link to the lists
        gares.Add(newSaveGare);
        lignes.Add(newln);
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