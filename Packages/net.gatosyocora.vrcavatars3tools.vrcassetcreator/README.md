# VRCAssetCreator (ver 1.2.0)

VRCSDK3に関連したアセットを作成します。

Projectウィンドウのアセットを作成したいフォルダで右クリックをして  
「Create>VRChat>...」で作成したいものを選択すると作成されます。  

VRCSDKに同封されているものを選択したフォルダに複製しています。

現在は以下のものを作成できます。  
「Create>VRChat>Controllers>...」：VRCSDKに同封されたAvatars3.0に関するAnimatorControllerら  

 * vrc_AvatarV3ActionLayer.controller : デフォルトでActionに設定されているAnimatorController
 * vrc_AvatarV3FaceLayer.controller : デフォルトでFXに設定されているAnimatorController
 * vrc_AvatarV3HandsLayer.controller : デフォルトでGestureに設定されているAnimatorController
 * vrc_AvatarV3HandsLayer2.controller : Gestureに設定候補のAnimatorController
 * vrc_AvatarV3IdleLayer.controller : デフォルトでAdditiveに設定されているAnimatorController
 * vrc_AvatarV3LocomotionLayer.controller : デフォルトでBaseに設定されているAnimatorController
 * vrc_AvatarV3SittingLayer.controller : デフォルトでSittingに設定されているAnimatorController
 * vrc_AvatarV3SittingLayer2.controller : Sittingに設定候補のAnimatorController
 * vrc_AvatarV3UtilityIKPose.controller : デフォルトでIKPoseに設定されているAnimatorController
 * vrc_AvatarV3UtilityTPose.controller : デフォルトでTPoseに設定されているAnimatorController

 「Create>VRChat>BlendTrees>...」：VRCSDKに同封されたAvatars3.0に関するBlendTreeら

 * New BlendTree : まだ未設定状態のBlendTree
 * vrc_StandingLocomotion : VRCSDKに同梱されている立ったときのモーションが登録されたBlendTree
 * vrc_CrouchingLocomotion : VRCSDKに同梱されているしゃがんだときのモーションが登録されたBlendTree
 * vrc_ProneLocomotion : VRCSDKに同梱されている伏せたときのモーションが登録されたBlendTree

 作成したBlendTreeを削除したときに以下のようなエラーログが出ることがありますが、    
 動作に特に問題はありません。  
 「MissingReferenceException: The object of type 'BlendTree' has been destroyed but you are still trying to access it.」  

## 更新履歴

### 1.2.0
  * VPM対応
### 1.1.1
  * VRChat Creator Companionに対応
### 1.1
  * BlendTreeも作成できるように
### 1.0.1
  * 意図してないAnimatorControllerから作成されることがある不具合を修正
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