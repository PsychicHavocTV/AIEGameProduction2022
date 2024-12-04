using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    [SerializeField]
    PlayerInput playerInput;
    [SerializeField]
    PlayerInput menuInput;

    public PauseController pauseHandler;
    public GameOverMenuController gameOverHandler;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.GameOver == true || GameManager.Instance.GamePaused == true)
        {
            Debug.Log("Game should pause.");
            if (playerInput.enabled == true)
            {
                playerInput.enabled = false;
            }
            if (menuInput.enabled == false)
            {
                menuInput.enabled = true;
            }
        }
        else
        {
            if (menuInput.enabled == true)
            {
                menuInput.enabled = false;
            }
            if (playerInput.enabled == false)
            {
                playerInput.enabled = true;
            }
        }

        //if (GameManager.Instance.GamePaused == true)
        //{
        //    gameOverHandler.enabled = false;
        //    pauseHandler.enabled = true;
        //}
        //else if (GameManager.Instance.GameOver == true)
        //{
        //    pauseHandler.enabled = false;
        //    gameOverHandler.enabled = true;
        //}
    }


    private void OnNavigateUP(InputValue value)
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (GameManager.Instance.GamePaused == true)
        {
            //pauseHandler.resetColor = false;
            if (pauseHandler.confirmationWindowShowing == false)
            {
                if (pauseHandler.currentIndex > 0)
                {
                    switch(pauseHandler.currentIndex)
                    {
                        case 2:
                            {
                                pauseHandler.quitButton.image.color = pauseHandler.quitButton.colors.normalColor;
                                pauseHandler.loadCheckpointButton.image.color = pauseHandler.loadCheckpointButton.colors.highlightedColor;
                                pauseHandler.currentIndex--;
                                break;
                            }
                        case 1:
                            {
                                pauseHandler.loadCheckpointButton.image.color = pauseHandler.loadCheckpointButton.colors.normalColor;
                                pauseHandler.resumeButton.image.color = pauseHandler.resumeButton.colors.highlightedColor;
                                pauseHandler.currentIndex--;
                                break;
                            }
                    }
                }
            }
        }
    }

    private void OnNavigateDOWN(InputValue value)
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (GameManager.Instance.GamePaused == true)
        {
            pauseHandler.resetColor = false;
            if (pauseHandler.confirmationWindowShowing == false)
            {
                if (pauseHandler.currentIndex < 2)
                {
                    switch (pauseHandler.currentIndex)
                    {
                        case 0:
                            {
                                pauseHandler.resumeButton.image.color = pauseHandler.resumeButton.colors.normalColor;
                                pauseHandler.loadCheckpointButton.image.color = pauseHandler.loadCheckpointButton.colors.highlightedColor;
                                pauseHandler.currentIndex++;
                                break;
                            }
                        case 1:
                            {
                                pauseHandler.loadCheckpointButton.image.color = pauseHandler.loadCheckpointButton.colors.normalColor;
                                pauseHandler.quitButton.image.color = pauseHandler.quitButton.colors.highlightedColor;
                                pauseHandler.currentIndex++;
                                break;
                            }
                    }
                }
            }
        }
    }


    private void OnConfirm(InputValue value)
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        //pauseHandler.resetColor = false;
        if (GameManager.Instance.GamePaused == true)
        {
            if (pauseHandler.confirmationWindowShowing == false)
            {
                switch (pauseHandler.currentIndex)
                {
                    case 0:
                        {
                            pauseHandler.ResumeGame();
                            break;
                        }
                    case 1:
                        {
                            pauseHandler.LoadData();
                            break;
                        }
                    case 2:
                        {
                            pauseHandler.QuitGame();
                            break;
                        }
                }
            }
        }
        else if (GameManager.Instance.GameOver == true)
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            Button loadButton = null;
            GameObject confirmWindow = null;
            loadButton = gameOverHandler.GetLoadButton(loadButton);
            confirmWindow = gameOverHandler.GetConfirmWindow(confirmWindow);
            if (gameOverHandler.confirmationWindowShowing == false)
            {
                switch (gameOverHandler.currentIndex)
                {
                    case 0:
                        {
                            
                            if (loadButton.enabled == true)
                            {
                                gameOverHandler.LoadData();
                            }
                            else if (loadButton.enabled == false)
                            {
                                gameOverHandler.QuitGame();
                            }
                            break;
                        }
                    case 1:
                        {
                            if (loadButton.enabled == true)
                            {
                                gameOverHandler.QuitGame();
                            }
                            else if (loadButton.enabled == false)
                            {
                                gameOverHandler.currentIndex = 0;
                            }
                            break;
                        }
                }
            }
            else if (gameOverHandler.confirmationWindowShowing == true)
            {
                switch (gameOverHandler.currentIndex)
                {
                    case 0:
                        {
                            
                            break;
                        }
                }
            }
        }
        //else if (pauseHandler.confirmationWindowShowing == true)
        //{
        //    if (pauseHandler.currentIndex == 0)
        //    {
        //        pauseHandler.QuitGame();
        //    }
        //    else if (pauseHandler.currentIndex == 1)
        //    {
        //        if (pauseHandler.confirmationWindow.activeSelf == true)
        //        {
        //            pauseHandler.currentIndex = 1;
        //            pauseHandler.confirmationWindow.SetActive(false);
        //            pauseHandler.confirmationWindowShowing = false;
        //        }
        //    }
        //}
    }
    private void OnNavigateLeft(InputValue value)
    {
    }

    private void OnNavigateRight(InputValue value)
    {
    }

    private void OnBack(InputValue value)
    {
        if (GameManager.Instance.GamePaused == true)
        {
            if (pauseHandler.confirmationWindowShowing == false)
            {
                pauseHandler.ResumeGame();
            }
        }
        //else if (pauseHandler.confirmationWindowShowing == true)
        //{
        //    pauseHandler.currentIndex = 1;
        //    pauseHandler.confirmationWindow.SetActive(false);
        //    pauseHandler.confirmationWindowShowing = false;
        //}
    }

    private void OnNavigateMouse(InputValue value)
    {
        if (Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (GameManager.Instance.GamePaused == true)
        {
            pauseHandler.resumeButton.image.color = pauseHandler.resumeButton.colors.normalColor;
            pauseHandler.loadCheckpointButton.image.color = pauseHandler.loadCheckpointButton.colors.normalColor;
            pauseHandler.quitButton.image.color = pauseHandler.quitButton.colors.normalColor;

            if (pauseHandler.confirmationWindow.activeSelf == true)
            {
                //pauseHandler.confirmationButtons[1].image.color = pauseHandler.confirmationButtons[1].colors.normalColor;
            }
        }
    }


    private void OnUnpause(InputValue value)
    {
        if (GameManager.Instance.GamePaused == true)
        {
            pauseHandler.ResumeGame();
        }
    }
}
