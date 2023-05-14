# AnimationBindingSeparater (ver 1.1.0)

AnimationClipからTransformを変更するキーを別のAnimationClipに分割します。

AnimationClipのInspectorの上部（AnimationClipの名前付近）を右クリックして  
「Separate the binding that changes Transform」を選択すると実行されます。  

以下のようなキーを(使用したAnimationClip名)_Transform.animに移動させます。

* Transformコンポーネントのプロパティを操作するもの（Position, Rotation, Scaleなど）
* AnimatorコンポーネントでHumanoidボーンの回転を操作するもの

## 注意点

これを使用したAnimationClipからTransformを変更するキーを削除して、  
新しく作成したAnimationClipに追加します。  
どちらも含まれるAnimationClipが必要な場合は、  
使用前にAnimationClipを複製して複製したほうに使用してください。  

## 更新履歴

### ver1.1.0
  * VPM対応
### ver1.0
  * ツールを作成

----------------------------------------------------

# 利用規約

本規約は本商品に含まれるすべてのスクリプトやファイルに共通で適用されるものとする。  
本商品を使用したことによって生じた問題に関してはgatosyocoraは一切の責任を負わない。  

## スクリプト

本スクリプトはMITライセンスで運用される。  
著作権はgatosyocoraに帰属する。  

-----------------------------------------------------
ご意見, ご要望があればTwitter: @gatosyocoraまで