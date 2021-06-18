using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCircle : MonoBehaviour {

    [SerializeField] private Animation animation;

    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Start playing the animation
    /// </summary>
    public void StartLoadingCircle() {
        animation.gameObject.SetActive(true);
        animation.Play();
    }
    /// <summary>
    /// stop playing the animation
    /// </summary>
    public void StopLoadingCircle() {
        animation.gameObject.SetActive(false);
        animation.Stop();
    }
}
