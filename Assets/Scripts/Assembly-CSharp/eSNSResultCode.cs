public enum eSNSResultCode
{
	NONE = -1,
	SUCCESS = 0,
	LOGIN_CANCEL = 2,
	NOT_SUPPORTED_IN_GUEST_MODE = 8,
	SUCCESS_NOT_VERIFIED = 10,
	UNDER_MAINTENANCE = -9788,
	KAServerErrorNotAuthorized = -1000,
	ERROR = 500,
	LOWER_AGE_LIMIT = -451,
	KAServerErrorInvaildGrant = 400,
	INVALID_PUSH_TOKEN = -200,
	INSUFFICIENT_PERMISSION = -100,
	INVITE_LIMIT = -32,
	EXCEED_INVITE_CHAT_LIMIT = -31,
	INVITE_MESSAGE_BLOCKED = -17,
	MESSAGE_BLOCK_USER = -16,
	CHAT_NOT_FOUND = -15,
	UNSUPPORTED_DEVICE = -14,
	UNREGISTERD_USER = -13,
	INVALID_REQUEST = -12,
	DEACTIVATED_USER = -11,
	NOT_FOUND = -10,
	CLOSEDIALOG = 100000,
	UNKNOWN = 99999
}
