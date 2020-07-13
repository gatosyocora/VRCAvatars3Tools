本セットに含まれる各種Editor拡張に関しての説明と利用規約を記載しています。

----------------------------------------------------
〇AnimatorControllerCombiner (ver 1.0.3)
AnimatorControllerのLayerとParameterを他のAnimatorControllerにコピーすることで合成します

「VRCAvatars3Tools > AnimatorControllerCombiner」で以下の機能を持つウィンドウが開きます

Src AnimatorController : コピー元AnimatorController
Dst AnimatorController : コピー先AnimatorController

「Combine」を押すとSourceAnmatorControllerのLayerとParameterがDestinationAnimatorControllerにコピーされます。

・既知の不具合
StateBehaviourを持つStateを含むLayerをコピーするとエラーが表示されます（動作には問題ない感じがします）

・更新履歴
ver1.0.3
	* SubStateMachineに対応
ver1.0.2 
	* 一番上のLayerのweight値が正しくコピーされていない不具合を修正
	* StateBehaviourのパラメータが正しくコピーされていない不具合を修正
	* Unityを再起動するとコピーしたLayer情報が消えてしまう不具合を修正
ver1.0.1
	* コピー先のLayerを削除するとコピー元のLayerのStateが削除される不具合を修正
ver1.0
	* ツールを作成

----------------------------------------------------

●利用規約
本規約は本商品に含まれるすべてのスクリプトやファイルに共通で適用されるものとする。
本商品を使用したことによって生じた問題に関してはgatosyocoraは一切の責任を負わない。

・スクリプト
本スクリプトはMITライセンスで運用される。
著作権はgatosyocoraに帰属する。

・Animationファイル
また、同封されているAnimationファイルはパラメータの一部を含め、商用利用・改変・二次配布を許可する。
その際には作者名や配布元等は記載しなくてもよい。
しかし、本Animationファイルの使用や配布により生じた問題等に関しては作者は一切の責任を負わない。

-----------------------------------------------------
ご意見, ご要望があればTwitter: @gatosyocoraまで