# Styly初挑戦

## バージョン情報

| 諸々名前 | バージョン |
|:-:|:-:|
|  Unity | 2019.3.6f1  |
|  STYLY Plugin | 1.3.0  |

---

## やること
- シーンのアップロード、公開などの基本操作習得  
- 簡単なインタラクションの習得  
  - 物を動かす、座標を移動させる
  - 物を持つ
  - 触ると動く、当たり判定的な  
  - AをBに接触させるとCが起こる  
- PlayMakerの習得
  - カメラの位置の取得、それに応じた処理
- 必要なShaderが問題なく動くか確認  

---

## 作りたい物(案)

### ■リモートワーク in VR
VR空間の中でVR空間に入るという体験を構築する。まあ割とありがち。  
VR空間内でHMDを被らせるのは普通ならやらないダサいUXだが、それをあえてやる。  
伝えたいメッセージは「リモートワーク」、「コロナ禍での真の働き方改革」。  

**【流れ、イメージ】**  
PCの前にHMDが置いてある。PCには会議への招待ダイアログが出ている。  
HMDを被る(頭に接触させる)と別の空間にワープする。
移動先は会議室。そこでは同僚や部下、上司がアバターで参加しており、仕事の打ち合わせを行う。その空間内には時間の概念があり、会議の終了時間になると自動的にワープで自宅に転送される。  

**【必要な素材】**
- 簡素な自宅の部屋
- HMDの3Dモデル
- PCの3Dモデル
- 会議室の3Dモデル
- ワープ演出、エフェクト
- 会議の音、人の声、ワープ音などサウンド

■候補  
[Simple Office](https://assetstore.unity.com/packages/3d/props/interior/simple-office-28730?locale=ja-JP)  
[Home Furniture Pack](https://assetstore.unity.com/packages/3d/props/furniture/home-furniture-pack-137357)  
[HQ ArchViz Conference Room](https://assetstore.unity.com/packages/3d/environments/hq-archviz-conference-room-127663)  

---

## 参考リンク
- [UnityからSTYLYにプレハブとシーンをアップロードする方法](https://styly.cc/ja/manual/unity-asset-uploader/)  
- [STYLY インタラクションSDK マニュアル](https://styly.cc/ja/manual/interaction_sdk_manual/)  
- [【Unity入門】PlayMakerを使った初めてのゲーム作り 概要とインストール](https://styly.cc/ja/tips/unity-playmaker-game-beginner-1/)
- [【Unity】ゼロからわかるPlayMaker「超」入門 基礎知識](https://styly.cc/ja/tips/playmaker_coreconcepts/)


---

## 詰まったところ、疑問点など


### Editor上からVRでの見え方を確認したい  

下記で解決。  
[STYLYローカルプレビューツール 「UnityシーンをSTYLY StudioにアップロードせずにSTYLY VR アプリで動作確認する方法」](https://styly.cc/ja/manual/styly-local-preview-tool/)  

---

### STYLY_Attr_Equipmentで良い感じに物を持ちたい

実機上でPreviewするとコントローラーと同じ角度に追従した。  
Editor上のシミュレートは40度回転していた。  

```cs
private void MovePosition() 
{
  Vector3 mousePos = Input.mousePosition;
  mousePos.z = distanceFromCamera;

  moveTo = mainCamera.ScreenToWorldPoint(mousePos);
  transform.position = moveTo;
  transform.rotation = mainCamera.transform.rotation;
  transform.eulerAngles += new Vector3(-40,0,0 );
}
```

なので、親オブジェクトの配下に物を置いて持った時に気持ちよくなる角度を探した。  

■課題  
"持ったときに気持ちよくなる角度"と"最初に置いてて不自然じゃない角度"が違う　という点だけ直したい。  

■解決策  
親に一つ空のオブジェクトを用意。親にIsTriggerにチェックを入れたColliderを用意し、掴むようにした。Blenderで回転いじっても良いが、面倒だったのでこれで対応。  

![StylyDoc1](ReadMEImage/StylyDoc1.gif)

親の原点から子の3Dモデルの原点を少しずらして、右端を持つようにした。  
左利きは諦めた。**Q.右手でしか使えないようにできる？**  
![StylyDoc2](ReadMEImage/StylyDoc2.gif)

---

### Playerの原点
**Q.Zが-5なのは何故？**  
Editor上でMainCameraの位置を-5にしておくとゲームビュー及びシーンビューが実機転送時・PlayMode時と同様の位置関係になった。  
![StylyDoc3](ReadMEImage/StylyDoc3.PNG)
