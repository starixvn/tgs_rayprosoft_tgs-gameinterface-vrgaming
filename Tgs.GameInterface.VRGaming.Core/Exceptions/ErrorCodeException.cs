namespace Tgs.GameInterface.VRGaming.Core.Exceptions;

public class ErrorCodeException(int errorCode) : Exception(ConvertErrorMesaage(errorCode))
{
	private static string ConvertErrorMesaage(int code) => code switch
	{
		0 => "成功",
		1 => "版本错误",
		2 => "找不到对应的界面",
		3 => "系统错误",
		4 => "网络传送失败",
		5 => "加密失败",
		6 => "解密失败",
		7 => "没有此商户",
		8 => "没有此玩家",
		9 => "新增玩家失败",
		10 => "余额不足",
		11 => "URL 错误",
		12 => "此玩家被禁用",
		13 => "玩家钱包转点-种类错误",
		14 => "玩家钱包转点-订单号重复",
		15 => "玩家钱包转点-订单号内容错误",
		16 => "玩家钱包转点-订单建立时间过太久",
		17 => "玩家钱包转点-金额错误",
		18 => "新增玩家失败，失败原因是此玩家已经存在。",
		999 => "未知错误",
		_ => $"遊戲供應商未提供錯誤代碼{code}"
	};
}
