using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_MoveJoueur : MonoBehaviour
{
    // Joueur
    public float vitesse;
    public float vitesseRotation = 300f;
    // Caméra
    public Camera camJoueur;
    float rotaCamFinal = 0f;
    public float maxRotaVertiCam = 75f;

    private void Start()
    {
        camJoueur = Camera.main;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        Mouvement();
    }

    void Mouvement()
    {
        float mouvementJoueur;
        float strafeJoueur = Input.GetAxis("Horizontal") * vitesse * Time.deltaTime;
        float rotaHoriJoueur = Input.GetAxis("Mouse X") * vitesseRotation * Time.deltaTime;
        float inputRotaCam = Input.GetAxis("Mouse Y") * vitesseRotation * Time.deltaTime;
        // Boost
        if (Input.GetKey(KeyCode.LeftShift)) { mouvementJoueur = Input.GetAxis("Vertical") * (vitesse * 2) * Time.deltaTime; }
        else { mouvementJoueur = Input.GetAxis("Vertical") * vitesse * Time.deltaTime; }

        // Application mouvement
        transform.Translate(Vector3.forward * mouvementJoueur);
        transform.Translate(Vector3.right * strafeJoueur);
        transform.Rotate(Vector3.up * rotaHoriJoueur);
        
        // Application rotation de la caméra
        rotaCamFinal -= inputRotaCam;
        rotaCamFinal = Mathf.Clamp(rotaCamFinal, -maxRotaVertiCam, maxRotaVertiCam);
        camJoueur.transform.localEulerAngles = new Vector3(rotaCamFinal, 0f, 0f);
    }
}
