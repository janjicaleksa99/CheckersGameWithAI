using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseAlgorithm : MonoBehaviour
{
    private GameObject MinimaxDescription;
    private GameObject NegamaxDescription;
    private GameObject MinimaxAlphaBetaDescription;

    private void Awake() {
        MinimaxDescription = GameObject.Find("/Canvas/Minimax");
        MinimaxDescription.SetActive(false);
        NegamaxDescription = GameObject.Find("/Canvas/Negamax");
        NegamaxDescription.SetActive(false);
        MinimaxAlphaBetaDescription = GameObject.Find("/Canvas/MinimaxAlphaBeta");
        MinimaxAlphaBetaDescription.SetActive(false);
    }

    public void OnValueChanged(int selected) {
        switch (selected) {
            case 0:
                MinimaxDescription.SetActive(false);
                NegamaxDescription.SetActive(false);
                MinimaxAlphaBetaDescription.SetActive(false);
                break;
            case 1:
                AIAlgorithms.currentAIAlgorithm = new Minimax();
                MinimaxDescription.SetActive(true);
                NegamaxDescription.SetActive(false);
                MinimaxAlphaBetaDescription.SetActive(false);
                break;
            case 2:
                AIAlgorithms.currentAIAlgorithm = new NegaMax();
                MinimaxDescription.SetActive(false);
                NegamaxDescription.SetActive(true);
                MinimaxAlphaBetaDescription.SetActive(false);
                break;
            case 3:
                AIAlgorithms.currentAIAlgorithm = new MinimaxAlphaBeta();
                MinimaxDescription.SetActive(false);
                NegamaxDescription.SetActive(false);
                MinimaxAlphaBetaDescription.SetActive(true);
                break;
        }
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
