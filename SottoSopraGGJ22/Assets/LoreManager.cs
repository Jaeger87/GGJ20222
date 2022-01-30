using UnityEngine;
using UnityEngine.SceneManagement;

public class LoreManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_Slides;

    [SerializeField]
    private Animator[] m_Animators;

    private int m_SlideIndex = -1;
    private float m_LastInput = 0f;

    private void Start()
    {
        NextSlide();
    }

    private void Update()
    {
        if (Input.GetAxis("Fire2") != m_LastInput)
        {
            m_LastInput = Input.GetAxis("Fire2");
            if (m_LastInput != 0f)
            {
                NextSlide();
            }
        }
    }

    public void NextSlide()
    {
        if(m_SlideIndex >= 0) 
            m_Slides[m_SlideIndex].SetActive(false);

        m_SlideIndex++;

        if (m_SlideIndex < m_Slides.Length)
        {
            m_Slides[m_SlideIndex].SetActive(true);
            m_Animators[m_SlideIndex].SetBool("Entered", true);
        }
        else
        {
            m_Slides[m_Slides.Length - 1].SetActive(false);
            SceneManager.LoadScene("Loading");
        }
    }
}
