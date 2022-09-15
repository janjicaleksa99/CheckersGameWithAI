using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class SelectorManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static PieceManager pieceOnTheMove;
    public static PieceManager pieceToBeEatenUL;
    public static PieceManager pieceToBeEatenUR;
    public static PieceManager pieceToBeEatenBL;
    public static PieceManager pieceToBeEatenBR;
    private Vector3 oldPosition;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData) {
        Vector3 selectorPosition = transform.localPosition;
        Vector3 piecePosition = pieceOnTheMove.transform.localPosition;

        float f = (selectorPosition.x - piecePosition.x) * 1000f;
        int xDiff = Mathf.RoundToInt(f);
        f = (selectorPosition.z - piecePosition.z) * 1000f;
        int zDiff = Mathf.RoundToInt(f);

        Animator animator = pieceOnTheMove.GetComponentInParent<Animator>();
        GameBoard gameBoard = GameObject.Find("/CheckerBoard1/CheckerBoard1").GetComponent<GameBoard>();

        if (xDiff == 250 && zDiff == 250) { // UpperLeftMove1
            animator.runtimeAnimatorController = AnimatorController.UpperLeftMove1;
        }
        else if (xDiff == 500 && zDiff == 500) { // UpperLeftMove2
            animator.runtimeAnimatorController = AnimatorController.UpperLeftMove2;
            pieceToBeEatenUL.gameObject.SetActive(false);
            gameBoard.removePiece(pieceToBeEatenUL.getPiece());
            PieceManager.alreadyAte = true;
        }
        else if (xDiff == 250 && zDiff == -250) { // UpperRightMove1
            animator.runtimeAnimatorController = AnimatorController.UpperRightMove1;
        }
        else if (xDiff == 500 && zDiff == -500) { // UpperRightMove2
            animator.runtimeAnimatorController = AnimatorController.UpperRightMove2;
            pieceToBeEatenUR.gameObject.SetActive(false);
            gameBoard.removePiece(pieceToBeEatenUR.getPiece());
            PieceManager.alreadyAte = true;
        }
        else if (xDiff == -250 && zDiff == 250) { // BottomLeftMove1
            animator.runtimeAnimatorController = AnimatorController.BottomLeftMove1;
        }
        else if (xDiff == -500 && zDiff == 500) { // BottomLeftMove2
            animator.runtimeAnimatorController = AnimatorController.BottomLeftMove2;
            pieceToBeEatenBL.gameObject.SetActive(false);
            gameBoard.removePiece(pieceToBeEatenBL.getPiece());
            PieceManager.alreadyAte = true;
        }
        else if (xDiff == -250 && zDiff == -250) { // BottomRightMove1
            animator.runtimeAnimatorController = AnimatorController.BottomRightMove1;
        }
        else if (xDiff == -500 && zDiff == -500) { // BottomRightMove2
            animator.runtimeAnimatorController = AnimatorController.BottomRightMove2;
            pieceToBeEatenBR.gameObject.SetActive(false);
            gameBoard.removePiece(pieceToBeEatenBR.getPiece());
            PieceManager.alreadyAte = true;
        }

        AnimatorController.currentAnimatiorController = animator.runtimeAnimatorController;

        oldPosition = piecePosition;
    }

    public void OnPointerUp(PointerEventData eventData) {
        Vector3 selectorPosition = transform.localPosition;
        pieceOnTheMove.setNewPosition(selectorPosition);
        pieceOnTheMove.getPiece().setLocalPosition(selectorPosition);
        pieceOnTheMove.getGameBoard().movePiece(oldPosition, pieceOnTheMove.getPiece());
        pieceOnTheMove.getPiece().turnOffSelectors();
    }
}
