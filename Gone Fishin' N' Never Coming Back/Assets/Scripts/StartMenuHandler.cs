using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuHandler : MonoBehaviour
{
   public void StartGame()
   {
      
       SceneManager.LoadScene("FirstWorld");
   }
   public void QuitGame()
   {
       // Quit the application
       Application.Quit();

   }
}
