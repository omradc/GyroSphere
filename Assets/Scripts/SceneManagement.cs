using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
	public class SceneManagement : MonoBehaviour
	{

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if(Input.GetMouseButton(2))
			{
               SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
	}
}