using UnityEngine;

public class GhostVisual : MonoBehaviour
{
    [Header("# Face")]
    public GameObject faces;
    public GameObject Stand;
    public GameObject Happy;
    public GameObject Angry;

    [Header("# Ascend")]
    public GameObject Ascend;
    public Animator AscendAnim;

    public enum Face {  Stand, Happy, Angry  }

    void Awake()
    {
        if (Ascend != null && AscendAnim == null)
        {
            AscendAnim = Ascend.GetComponent<Animator>();
        }
    }

    public void ShowFace(Face face)
    {
        if (Ascend != null)
            Ascend.SetActive(false);
        if (faces != null)
            faces.SetActive(true);
        Stand.SetActive(face == Face.Stand);
        Happy.SetActive(face == Face.Happy);
        Angry.SetActive(face == Face.Angry);
    }

    public void PlayAscend()
    {
        Stand.SetActive(false);
        Happy.SetActive(false);
        Angry.SetActive(false);

        Ascend.SetActive(true);
        if (AscendAnim != null)
        {
            AscendAnim.Play("Ascend", 0, 0f);
        }
    }
}
