using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets
{
    public class TitleController : MonoBehaviour
    {
        [SerializeField] int NextScene;

        public void ButtonClicked()
        {
            SceneManager.LoadScene(NextScene);
        }
    }
}
