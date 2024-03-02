using TMPro;
using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
    TMP_Text tmp_text;
    public StatCategory category;
    [SerializeField] string precision = "F7";
    void Start()
    {
        tmp_text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tmp_text != null)
        {
            if (category != null)
            {
                tmp_text.text = category.name + "\n" + category.GetAverage().ToString(precision);
            } else
            {
                print("category not found");
            }
        }
        else
        {
            print("tmp_text not found");
        }
    }
}
