# AnimatorControllerCombiner (ver 1.2.0)

AnimatorControllerのLayerとParameterを他のAnimatorControllerにコピーすることで合成します

「VRCAvatars3Tools > AnimatorControllerCombiner」で以下の機能を持つウィンドウが開きます

Src AnimatorController : コピー元AnimatorController
Dst AnimatorController : コピー先AnimatorController

「Combine」を押すとSourceAnmatorControllerのLayerとParameterがDestinationAnimatorControllerにコピーされます。
チェックが入っているLayerやParameterがコピーされます。
同名のLayerがある場合、Layerが別名でコピーされます。
同名のParameterがある場合、そのParameterはコピーされません。

## 更新履歴
### 1.2.0
  * VPM対応
### 1.1.2
  * 内部的な軽微な変更
### 1.1.1
  * VRCSDK3-AVATAR-2022.06.03.00.04_Public.unitypackageに対応
### 1.1
  * LayerやParameterを選択してコピーできるように
### 1.0.3.3
  * VRCSDK3-AVATAR-2020.08.06.16.30_Public.unitypackageに対応
### 1.0.3.2
  * VRCSDK3-AVATAR-2020.07.30.09.18_Public.unitypackageに対応
### 1.0.3.1
  * VRCSDK3-AVATAR-2020.07.14.20.50_Public.unitypackageに対応
  * VRCAnimatorSetViewを廃止, VRCAnimatorRemeasureAvatarとVRCAnimatorTemporaryPoseSpaceを追加
### 1.0.3
  * SubStateMachineに対応
### 1.0.2 
  * 一番上のLayerのweight値が正しくコピーされていない不具合を修正
  * StateBehaviourのパラメータが正しくコピーされていない不具合を修正
  * Unityを再起動するとコピーしたLayer情報が消えてしまう不具合を修正
### 1.0.1
  * コピー先のLayerを削除するとコピー元のLayerのStateが削除される不具合を修正
### 1.0
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