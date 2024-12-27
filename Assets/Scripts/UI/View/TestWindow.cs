using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class TestWindow : BaseWindow
// {
//     public TestWindow()
//     {
//         resName = "UI/Window/TestWindow";
//         resident = true;
//         selfType = WindowType.TestWindow;
//         scenesType = ScenesType.KB;
//     }

//     protected override void Awake()
//     {
//         base.Awake();
//     }

//     protected override void OnAddListener()
//     {
//         base.OnAddListener();
//     }

//     protected override void OnDisable()
//     {
//         base.OnDisable();
//     }

//     protected override void OnEnable()
//     {
//         base.OnEnable();
//     }

//     protected override void OnRemoveListener()
//     {
//         base.OnRemoveListener();
//     }

//     protected override void RegisterUIEvent()
//     {
//         base.RegisterUIEvent();
//         foreach(var button in buttonList)
//         {
//             switch(button.name)
//             {
//                 case "TestButton":
//                     button.onClick.AddListener(OnTestButton);
//                     break;
//             }
//         }
//     }

//     public override void Update(float deltaTime)
//     {
//         base.Update(deltaTime);
//         if(Input.GetKeyDown(KeyCode.C))
//         {
//             Close();
//         }
//     }


//     private void OnTestButton()
//     {
//         Debug.Log("点击事件产生成功");
//     } 
// }
