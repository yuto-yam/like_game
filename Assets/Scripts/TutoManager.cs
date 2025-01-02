using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutoManager : MonoBehaviour
{
    public InputField inputField; // タイトルに置いたInputField

    public TextAsset TutoText;          // .txtで保存したチュートリアルテキスト
    [SerializeField] string[] TutoList;      // TutoTextを分割したリスト
    public List<Sprite> TutoImageList; // チュートリアルに使う画像リスト

    [SerializeField] GameObject MainPanel; // テキストを置いているパネル
    [SerializeField] GameObject MainText;  // 説明を出すためのテキストオブジェクト
    private UnityEngine.UI.Text maintext;

    [SerializeField] GameObject TitlePanel;

    [SerializeField] GameObject ImageObject; // 画像を差し替えるオブジェクト
    private SpriteRenderer spriteRenderer;

    public int i = 0;
    private int MaxTextWidth = 0; // チュートリアルテキストの最大長
    private Vector2 StandardPixelSize;


    // Start is called before the first frame update
    void Start()
    {
        LoadTutoTextData(TutoText);
        // テキストの最大長を計算
        MaxTextWidth = TutoList.Max(text => text.Length);

        maintext = MainText.GetComponent<UnityEngine.UI.Text>();
        MainPanel.SetActive(false);

        spriteRenderer = ImageObject.GetComponent<SpriteRenderer>();
        StandardPixelSize = spriteRenderer.sprite.rect.size;
    }

    // Update is called once per frame
    void Update()
    {
        if (inputField.text != "プレイヤー名(全角5文字まで)を入れてEnter" && inputField.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                TitlePanel.SetActive(false);
                MainPanel.SetActive(true);
            }

            // RightArrowが押された場合にiを変更
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (i == 5) // iが5の時だけシーン遷移を行う
                {
                    SceneManager.LoadScene("Maze");
                }
                else
                {
                    i = Mathf.Min(i + 1, TutoList.Length - 1);
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                i = Mathf.Max(0, i - 1);
            }

            // iの値に応じて表示する内容を更新
            if (i > 0)
            {
                maintext.text = $"{TutoList[i].PadLeft(MaxTextWidth)} <←/→>";
                spriteRenderer.sprite = TutoImageList[i];
                ResizeImage(ImageObject, StandardPixelSize);
                ImageObject.transform.localScale = new Vector3(75f, 75f, 1);
            }
            else
            {
                maintext.text = $"{TutoList[0].PadLeft(MaxTextWidth)} <  /→>";
                spriteRenderer.sprite = TutoImageList[i];
                ResizeImage(ImageObject, StandardPixelSize);
                ImageObject.transform.localScale = new Vector3(75f, 75f, 1);
            }
        }
    }

    /* 以下関数の定義 */
    // 定義した名前に入力された名前を登録
    public void SaveInputedName()
    {
        ParameterDifiner.InputedName = inputField.text;
    }

    // テキストファイルを分割
    public void LoadTutoTextData(TextAsset textAsset)
    {
        // 列ごとにtxtを読み込み
        TutoList = textAsset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);      
    }

    // 画像サイズを固定値で
    public void ResizeImage(GameObject targetObject, Vector2 targetPixelSize)
    {
        SpriteRenderer targetSpriteRenderer = targetObject.GetComponent<SpriteRenderer>();

        if (targetSpriteRenderer != null && targetSpriteRenderer.sprite != null)
        {
            // 元のスプライトのピクセルサイズを取得
            Vector2 originalPixelSize = targetSpriteRenderer.sprite.rect.size;

            // 新しいサイズに基づいてスケールを計算
            float widthRatio = targetPixelSize.x / originalPixelSize.x;
            float heightRatio = targetPixelSize.y / originalPixelSize.y;

            // 新しいスケールを設定（スプライトのピクセルサイズに基づいて）
            targetObject.transform.localScale = new Vector3(widthRatio, heightRatio, 1f);
        }
        else
        {
            UnityEngine.Debug.LogWarning("SpriteRendererまたはSpriteが存在しません。");
        }
    }
}
