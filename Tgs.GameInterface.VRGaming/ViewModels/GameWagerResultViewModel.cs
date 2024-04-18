namespace Tgs.GameInterface.VRGaming.ViewModels;

public record GameWagerResultViewModel(
	string ExternalRoundId,
	string ExternalWagerId,
	string ExternalGameId,
	string PlayerCode,
	bool IsClosed,
	bool IsCanceled,
	bool IsResettle,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	DateTimeOffset ExternalResettleOn,
	decimal Bet,
	decimal Settle,
	decimal WinLoss,
	string AdditionalData);
