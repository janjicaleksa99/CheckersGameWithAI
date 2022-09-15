using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour, IPointerDownHandler {

    public void OnPointerDown(PointerEventData eventData) {
        AIAlgorithms.currentAIAlgorithm = null;
        AIAlgorithms.possibleAIMoves.Clear();
        AIAlgorithms.depthOfTree = -1;
        PieceManager.resetBools();
        SceneManager.LoadScene("GameMode");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
