using UnityEngine;
using UnityEngine.UI;

public class PlayerNameplate : MonoBehaviour {

    [SerializeField]
    private Text usernameText;

    [SerializeField]
    private RectTransform healthBarFill;

    [SerializeField]
    private Player player;

    private Vector3 nameplateOffset;

    private void Start() {
        nameplateOffset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update () {
        usernameText.text = player.username;
        healthBarFill.localScale = new Vector3(player.GetHealthPct(), 1f, 1f);

        // Keep the nameplate on the same place (facing up) relative to the rocket
        transform.position = player.transform.position + nameplateOffset;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
}
