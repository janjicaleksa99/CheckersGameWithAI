using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewAIMove : MonoBehaviour
{
    private GameBoard gameBoard;
    private GameObject AISelector;
    private PieceManager oldPiece = null;

    public void turnSpotlightOff() {
        if (oldPiece != null)
            oldPiece.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
    }

    public void OnValueChanged(int selected) {
        turnSpotlightOff();

        AIVals aiVals = (AIVals) AIAlgorithms.possibleAIMoves[selected];
        Move move = aiVals.move;
        Piece piece = gameBoard.getPiece(move.oldPosition.i, move.oldPosition.j);


        oldPiece = piece.getPieceManager();
        if (oldPiece != null)
            oldPiece.GetComponent<Renderer>().material.color = new Color(0.9716981f, 0.4037762f, 0.06875221f, 1);

        Vector3 selectorPosition = piece.getLocalPosition();

        int iDiff = move.newPosition.i - move.oldPosition.i;
        int jDiff = move.newPosition.j - move.oldPosition.j;

        if (iDiff == -1 && jDiff == -1) { // UpperLeftMove1
            selectorPosition.x += 0.25f;
            selectorPosition.z += 0.25f;
        }
        else if (iDiff == -2 && jDiff == -2) { // UpperLeftMove2
            selectorPosition.x += 0.5f;
            selectorPosition.z += 0.5f;
        }
        else if (iDiff == -1 && jDiff == 1) { // UpperRightMove1
            selectorPosition.x += 0.25f;
            selectorPosition.z -= 0.25f;
        }
        else if (iDiff == -2 && jDiff == 2) { // UpperRightMove2
            selectorPosition.x += 0.5f;
            selectorPosition.z -= 0.5f;
        }
        else if (iDiff == 1 && jDiff == -1) { // BottomLeftMove1
            selectorPosition.x -= 0.25f;
            selectorPosition.z += 0.25f;
        }
        else if (iDiff == 2 && jDiff == -2) { // BottomLeftMove2
            selectorPosition.x -= 0.5f;
            selectorPosition.z += 0.5f;
        }
        else if (iDiff == 1 && jDiff == 1) { // BottomRightMove1
            selectorPosition.x -= 0.25f;
            selectorPosition.z -= 0.25f;
        }
        else if (iDiff == 2 && jDiff == 2) { // BottomRightMove2
            selectorPosition.x -= 0.5f;
            selectorPosition.z -= 0.5f;
        }

        AISelector.transform.localPosition = selectorPosition;
        AISelector.SetActive(true);
    }

    private void Awake() {
        AISelector = GameObject.Find("/CheckerBoard1/CheckerBoard1/AISelector");
        gameBoard = GameObject.Find("/CheckerBoard1/CheckerBoard1").GetComponent<GameBoard>();
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
