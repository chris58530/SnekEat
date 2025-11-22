# 架構生成指令說明

當我說：「**幫我產生 [Name] 架構**」時，請執行以下動作：

1.  **產生 View 腳本**：
    *   檔名：`[Name]View.cs`
    *   路徑：`Assets/Scripts/View/`
    *   內容：繼承 `BaseView<[Name]ViewMediator>`，並包含基本的 `Awake` 或 `Start` 方法。

2.  **產生 Mediator 腳本**：
    *   檔名：`[Name]ViewMediator.cs`
    *   路徑：`Assets/Scripts/Mediator/`
    *   內容：繼承 `BaseMediator<[Name]View>`。

## 範例：產生 Score 架構 (僅供參考，不需產生實體檔案)

**ScoreView.cs**
```csharp
// 僅供參考，不需產生實體腳本
using UnityEngine;
using Core.MVC;

public class ScoreView : BaseView<ScoreViewMediator>
{
    // TODO: Add UI references here
}
```

**ScoreViewMediator.cs**
```csharp
// 僅供參考，不需產生實體腳本
using Core.MVC;

public class ScoreViewMediator : BaseMediator<ScoreView>
{
    // TODO: Add logic here
}
```

# MVC 架構驗證規則

在執行任何程式碼生成或修改時，請檢查是否違反以下規則：

1.  **Proxy 限制**：
    *   Proxy **不可** Inject 其他 Proxy。資料層應保持獨立。
2.  **View 限制**：
    *   View **不可** 發送事件 (不能呼叫 `listener.BroadCast`)。所有互動須透過 Mediator 處理。
    *   View **可以** Inject Mediator。
3.  **綁定關係**：
    *   View 與 Mediator 是唯一互相綁定的對象 (View Inject Mediator，Mediator 持有 View)。
