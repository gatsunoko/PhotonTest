using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

// MonoBehaviourPunCallbacksを継承すると、photonViewプロパティが使えるようになる
public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
  private SpriteRenderer spriteRenderer;
  private float hue = 0f; // 色相値
  private bool isMoving = false; // 移動中フラグ

  private void Awake() {
    spriteRenderer = GetComponent<SpriteRenderer>();

    ChangeBodyColor();
  }

  private void Update() {
    if (photonView.IsMine) {
      var direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
      var dv = 6f * Time.deltaTime * direction;
      transform.Translate(dv.x, dv.y, 0f);

      // 移動中なら色相値を変化させていく
      isMoving = direction.magnitude > 0f;
      if (isMoving) {
        hue = (hue + Time.deltaTime) % 1f;
      }

      ChangeBodyColor();
    }
  }

  // データを送受信するメソッド
  void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    if (stream.IsWriting) {
      // 自身側が生成したオブジェクトの場合は
      // 色相値と移動中フラグのデータを送信する
      stream.SendNext(hue);
      stream.SendNext(isMoving);
    }
    else {
      // 他プレイヤー側が生成したオブジェクトの場合は
      // 受信したデータから色相値と移動中フラグを更新する
      hue = (float)stream.ReceiveNext();
      isMoving = (bool)stream.ReceiveNext();

      ChangeBodyColor();
    }
  }

  private void ChangeBodyColor() {
    float h = hue;
    float s = 1f;
    float v = (isMoving) ? 1f : 0.5f;
    spriteRenderer.color = Color.HSVToRGB(h, s, v);
  }
}
