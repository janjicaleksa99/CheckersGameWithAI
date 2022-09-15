using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContinueMoveButton : MonoBehaviour, IPointerDownHandler
{
    private GameBoard gameBoard;
    private GameObject AIMoves;

    public void OnPointerDown(PointerEventData eventData) {
        AIMoves.SetActive(false);
        gameBoard.continueAIMove();
    }

    private void Awake() {
        gameBoard = GameObject.Find("/CheckerBoard1/CheckerBoard1").GetComponent<GameBoard>();
        AIMoves = GameObject.Find("/AIMoves");
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
