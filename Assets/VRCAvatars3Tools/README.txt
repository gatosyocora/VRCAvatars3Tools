本セットに含まれる各種Editor拡張に関しての説明と利用規約を記載しています。

----------------------------------------------------
# AnimatorControllerCombiner (ver 1.1)

AnimatorControllerのLayerとParameterを他のAnimatorControllerにコピーすることで合成します

「VRCAvatars3Tools > AnimatorControllerCombiner」で以下の機能を持つウィンドウが開きます

Src AnimatorController : コピー元AnimatorController
Dst AnimatorController : コピー先AnimatorController

「Combine」を押すとSourceAnmatorControllerのLayerとParameterがDestinationAnimatorControllerにコピーされます。
チェックが入っているLayerやParameterがコピーされます。
同名のLayerがある場合、Layerが別名でコピーされます。
同名のParameterがある場合、そのParameterはコピーされません。

## 既知の不具合

StateBehaviourを持つStateを含むLayerをコピーするとエラーが表示されます（動作には問題ない感じがします）

## 更新履歴

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
# ExpressionParametersCombiner (ver 1.0)

ExpressionParametersのParameterを他のExpressionParametersにコピーすることで合成します

「VRCAvatars3Tools > ExpressionParametersCombiner」で以下の機能を持つウィンドウが開きます

* Src ExpressionParameters : コピー元ExpressionParameters  
* Dst ExpressionParameters : コピー先ExpressionParameters  

「Combine」を押すとSrc ExpressionParametersのParameterがDst ExpressionParametersにコピーされます。  
チェックが入っているParameterがコピーされます。  
同名のParameterがある場合、そのParameterはコピーされません。  

## 更新履歴

### 1.0
  * ツールを作成

----------------------------------------------------
# VRCAvatarConverterTo3 (ver 1.3.3)

VRCSDK2で作成したアバター(Avatars2.0)をAvatars3.0のアバターに変換できます。

「VRCAvatars3Tools > VRCAvatarConverterTo3」で以下の機能を持つウィンドウが開きます

## 使い方

1. 「2.0 Avatar Prefab」の右にある◎を押して変換したいPrefabを選択します。（Avatars2.0のPrefab）
2. 設定される内容が表示されます。
3. 「Convert Avatar To 3.0」を選択して変換します。

「EN」「JP」を押すとUIの一部が英語/日本語に切り替わります。

## 注意点

 * EyeLookのEyesにあるRotationStatesは未設定です。必要に応じて各自設定してください。  
	使用しない場合は上部ボタンの「Disable」を押して、EyeLook自体を無効にしてもいいかもしれません。
 * EyeLookのEyelidsはBlink,LookingUp,LookingDownに最適なBlendShapeを決定できないので、  
	EyelidTypeはNoneにしています。使用する場合は変更して下さい。  
	Blendshapesを選択した場合はEyelidsMeshにBodyという名前のSkinnedMeshRendererが選択されるようにしています。

 * まだIdleAnimation, CustomSittingAnimsの変換はできていません。

## Avatars2.0からのアバター変換

1. Avatars2.0で作成したアバターをPrefabにして、それを含めたunitypackageを作成する
   * Prefabで右クリックしてExport packageするとよい
   * Export画面でVRCSDKやEditor拡張などはチェックを外してunitypackageに含まれないようにする
    (Editor拡張はEditorというフォルダが含まれているのでこれで見分けがつく）
2. Avatars3.0用に新しくプロジェクトを作成する
3. 2で作成したプロジェクトにAvatars3.0用のVRCSDKと本ツールと1で作成したunitypackageをインポートする
　　* 1で作成したunitypackageをインポートする前にアバターに必要なアセットを別途インポートしておく
   (ShaderやDynamicBoneなど)
4. 1で作成したunitypackageに含まれるPrefabを本ツールの「2.0 Avatar Prefab」に設定する

##使用ライブラリ

 * YamlDotNet for Unity
 Copyright (c) 2008, 2009, 2010, 2011, 2012, 2013, 2014 Antoine Aubry

## WriteDefaultsオフでの対応は以下のようなことをやっています

 * FX LayerのController(vrc_AvatarV3HandsLayerまたはvrc_AvatarV3HandsLayer2)の2番目に表情をリセットするレイヤー(ResetFace)を追加
 * FX LayerのLeft Hand, Right HandレイヤーにあるIdle(Idle 2)ステートに空のAnimationClipを設定
 Ryubanさん作成の以下のドキュメントの情報を参考に設定してあります。
 https://docs.google.com/document/d/1FNiyIl5RyVdfR3pgCBMoc501sCAHs4ZVCnJ6xpbwf6Q/preview#heading=h.45y4ogqf1vgx

## 更新履歴

### 1.3.3
  * VRCSDK2のプロジェクトにいれたときにエラーが出る不具合を修正
### 1.3.2
  * WriteDefaultsがOFFのSDK同封のAnimatorControllerに対応
  * VRCSDK2のVRCAvatarDescripterだったものを自動で削除するように(暫定対応)
### 1.3.1
  * 意図してないAnimatorControllerから作成されることがある不具合を修正
### 1.3
  * 性別に応じてSittingのアニメーションとHandsLayerのアニメーションを選択するように(PR @skishida)
  * まばたき干渉防止が設定されるように(PR @skishida)
  * Idleが設定されている場合はその手の形をデフォルトとして設定するように(PR @skishida)
  * EyeLookを無効にするように(RotationStatesが未設定だと白目になるアバターがあるため)
  * デフォルト言語を日本語に
### 1.2
  * UIを日本語に切り替えられるように
  * 一部の改変アバターでPrefabの情報が取得できない不具合を修正
  * CustomStandingAnimsが未設定のアバターに対応
  * fbxを選択したときにエラーメッセージがでるように
### 1.1.0.1
  * VRCSDK3-AVATAR-2020.08.06.16.30_Public.unitypackageに対応
### 1.1
  * Emoteも変換されるように
### 1.0.0.1
  * VRCSDK3-AVATAR-2020.07.30.09.18_Public.unitypackageに対応
### 1.0
  * ツールを作成

----------------------------------------------------
# VRCAssetCreator (ver 1.1)

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

### 1.1
  * BlendTreeも作成できるように
### 1.0.1
  * 意図してないAnimatorControllerから作成されることがある不具合を修正
### 1.0
  * ツールを作成

----------------------------------------------------
# AnimationBindingSeparater (ver 1.0)

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

### ver1.0
  * ツールを作成

----------------------------------------------------

# 利用規約

本規約は本商品に含まれるすべてのスクリプトやファイルに共通で適用されるものとする。  
本商品を使用したことによって生じた問題に関してはgatosyocoraは一切の責任を負わない。  

## スクリプト

本スクリプトはMITライセンスで運用される。  
著作権はgatosyocoraに帰属する。  

## Animationファイル

また、同封されているAnimationファイルはパラメータの一部を含め、商用利用・改変・二次配布を許可する。  
その際には作者名や配布元等は記載しなくてもよい。  
しかし、本Animationファイルの使用や配布により生じた問題等に関しては作者は一切の責任を負わない。  

-----------------------------------------------------
ご意見, ご要望があればTwitter: @gatosyocoraまで