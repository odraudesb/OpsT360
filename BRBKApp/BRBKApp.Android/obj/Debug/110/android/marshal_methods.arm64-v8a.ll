; ModuleID = 'obj\Debug\110\android\marshal_methods.arm64-v8a.ll'
source_filename = "obj\Debug\110\android\marshal_methods.arm64-v8a.ll"
target datalayout = "e-m:e-i8:8:32-i16:16:32-i64:64-i128:128-n32:64-S128"
target triple = "aarch64-unknown-linux-android"


%struct.MonoImage = type opaque

%struct.MonoClass = type opaque

%struct.MarshalMethodsManagedClass = type {
	i32,; uint32_t token
	%struct.MonoClass*; MonoClass* klass
}

%struct.MarshalMethodName = type {
	i64,; uint64_t id
	i8*; char* name
}

%class._JNIEnv = type opaque

%class._jobject = type {
	i8; uint8_t b
}

%class._jclass = type {
	i8; uint8_t b
}

%class._jstring = type {
	i8; uint8_t b
}

%class._jthrowable = type {
	i8; uint8_t b
}

%class._jarray = type {
	i8; uint8_t b
}

%class._jobjectArray = type {
	i8; uint8_t b
}

%class._jbooleanArray = type {
	i8; uint8_t b
}

%class._jbyteArray = type {
	i8; uint8_t b
}

%class._jcharArray = type {
	i8; uint8_t b
}

%class._jshortArray = type {
	i8; uint8_t b
}

%class._jintArray = type {
	i8; uint8_t b
}

%class._jlongArray = type {
	i8; uint8_t b
}

%class._jfloatArray = type {
	i8; uint8_t b
}

%class._jdoubleArray = type {
	i8; uint8_t b
}

; assembly_image_cache
@assembly_image_cache = local_unnamed_addr global [0 x %struct.MonoImage*] zeroinitializer, align 8
; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = local_unnamed_addr constant [240 x i64] [
	i64 120698629574877762, ; 0: Mono.Android => 0x1accec39cafe242 => 24
	i64 195258733703605363, ; 1: System.Net.Http.Formatting => 0x2b5b2c8a5b22c73 => 43
	i64 210515253464952879, ; 2: Xamarin.AndroidX.Collection.dll => 0x2ebe681f694702f => 63
	i64 232391251801502327, ; 3: Xamarin.AndroidX.SavedState.dll => 0x3399e9cbc897277 => 87
	i64 295915112840604065, ; 4: Xamarin.AndroidX.SlidingPaneLayout => 0x41b4d3a3088a9a1 => 88
	i64 634308326490598313, ; 5: Xamarin.AndroidX.Lifecycle.Runtime.dll => 0x8cd840fee8b6ba9 => 78
	i64 654518984046852588, ; 6: SdkApi.Core => 0x915518c56dcb9ec => 32
	i64 702024105029695270, ; 7: System.Drawing.Common => 0x9be17343c0e7726 => 5
	i64 720058930071658100, ; 8: Xamarin.AndroidX.Legacy.Support.Core.UI => 0x9fe29c82844de74 => 72
	i64 870603111519317375, ; 9: SQLitePCLRaw.lib.e_sqlite3.android => 0xc1500ead2756d7f => 37
	i64 872800313462103108, ; 10: Xamarin.AndroidX.DrawerLayout => 0xc1ccf42c3c21c44 => 69
	i64 940822596282819491, ; 11: System.Transactions => 0xd0e792aa81923a3 => 107
	i64 996343623809489702, ; 12: Xamarin.Forms.Platform => 0xdd3b93f3b63db26 => 101
	i64 1000557547492888992, ; 13: Mono.Security.dll => 0xde2b1c9cba651a0 => 117
	i64 1120440138749646132, ; 14: Xamarin.Google.Android.Material.dll => 0xf8c9a5eae431534 => 103
	i64 1301485588176585670, ; 15: SQLitePCLRaw.core => 0x120fce3f338e43c6 => 36
	i64 1315114680217950157, ; 16: Xamarin.AndroidX.Arch.Core.Common.dll => 0x124039d5794ad7cd => 58
	i64 1400031058434376889, ; 17: Plugin.Permissions.dll => 0x136de8d4787ec4b9 => 31
	i64 1425944114962822056, ; 18: System.Runtime.Serialization.dll => 0x13c9f89e19eaf3a8 => 48
	i64 1518315023656898250, ; 19: SQLitePCLRaw.provider.e_sqlite3 => 0x151223783a354eca => 38
	i64 1624659445732251991, ; 20: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 0x168bf32877da9957 => 56
	i64 1628611045998245443, ; 21: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 0x1699fd1e1a00b643 => 80
	i64 1636321030536304333, ; 22: Xamarin.AndroidX.Legacy.Support.Core.Utils.dll => 0x16b5614ec39e16cd => 73
	i64 1731380447121279447, ; 23: Newtonsoft.Json => 0x18071957e9b889d7 => 27
	i64 1743969030606105336, ; 24: System.Memory.dll => 0x1833d297e88f2af8 => 42
	i64 1795316252682057001, ; 25: Xamarin.AndroidX.AppCompat.dll => 0x18ea3e9eac997529 => 57
	i64 1836611346387731153, ; 26: Xamarin.AndroidX.SavedState => 0x197cf449ebe482d1 => 87
	i64 1865037103900624886, ; 27: Microsoft.Bcl.AsyncInterfaces => 0x19e1f15d56eb87f6 => 22
	i64 1875917498431009007, ; 28: Xamarin.AndroidX.Annotation.dll => 0x1a08990699eb70ef => 54
	i64 1981742497975770890, ; 29: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 0x1b80904d5c241f0a => 79
	i64 1986553961460820075, ; 30: Xamarin.CommunityToolkit => 0x1b91a84d8004686b => 96
	i64 2040001226662520565, ; 31: System.Threading.Tasks.Extensions.dll => 0x1c4f8a4ea894a6f5 => 119
	i64 2133195048986300728, ; 32: Newtonsoft.Json.dll => 0x1d9aa1984b735138 => 27
	i64 2136356949452311481, ; 33: Xamarin.AndroidX.MultiDex.dll => 0x1da5dd539d8acbb9 => 84
	i64 2165725771938924357, ; 34: Xamarin.AndroidX.Browser => 0x1e0e341d75540745 => 61
	i64 2262844636196693701, ; 35: Xamarin.AndroidX.DrawerLayout.dll => 0x1f673d352266e6c5 => 69
	i64 2284400282711631002, ; 36: System.Web.Services => 0x1fb3d1f42fd4249a => 112
	i64 2329709569556905518, ; 37: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 0x2054ca829b447e2e => 76
	i64 2335503487726329082, ; 38: System.Text.Encodings.Web => 0x2069600c4d9d1cfa => 49
	i64 2337758774805907496, ; 39: System.Runtime.CompilerServices.Unsafe => 0x207163383edbc828 => 46
	i64 2470498323731680442, ; 40: Xamarin.AndroidX.CoordinatorLayout => 0x2248f922dc398cba => 64
	i64 2479423007379663237, ; 41: Xamarin.AndroidX.VectorDrawable.Animated.dll => 0x2268ae16b2cba985 => 91
	i64 2489738558632930771, ; 42: Acr.UserDialogs.dll => 0x228d540722e8add3 => 14
	i64 2497223385847772520, ; 43: System.Runtime => 0x22a7eb7046413568 => 47
	i64 2547086958574651984, ; 44: Xamarin.AndroidX.Activity.dll => 0x2359121801df4a50 => 53
	i64 2592350477072141967, ; 45: System.Xml.dll => 0x23f9e10627330e8f => 51
	i64 2624866290265602282, ; 46: mscorlib.dll => 0x246d65fbde2db8ea => 25
	i64 2783046991838674048, ; 47: System.Runtime.CompilerServices.Unsafe.dll => 0x269f5e7e6dc37c80 => 46
	i64 2801558180824670388, ; 48: Plugin.CurrentActivity.dll => 0x26e1225279a4e0b4 => 28
	i64 2960931600190307745, ; 49: Xamarin.Forms.Core => 0x2917579a49927da1 => 98
	i64 3017704767998173186, ; 50: Xamarin.Google.Android.Material => 0x29e10a7f7d88a002 => 103
	i64 3289520064315143713, ; 51: Xamarin.AndroidX.Lifecycle.Common => 0x2da6b911e3063621 => 75
	i64 3303437397778967116, ; 52: Xamarin.AndroidX.Annotation.Experimental => 0x2dd82acf985b2a4c => 55
	i64 3311221304742556517, ; 53: System.Numerics.Vectors.dll => 0x2df3d23ba9e2b365 => 45
	i64 3325875462027654285, ; 54: System.Runtime.Numerics => 0x2e27e21c8958b48d => 10
	i64 3522470458906976663, ; 55: Xamarin.AndroidX.SwipeRefreshLayout => 0x30e2543832f52197 => 89
	i64 3531994851595924923, ; 56: System.Numerics => 0x31042a9aade235bb => 44
	i64 3571415421602489686, ; 57: System.Runtime.dll => 0x319037675df7e556 => 47
	i64 3609787854626478660, ; 58: Plugin.CurrentActivity => 0x32188aeda587da44 => 28
	i64 3716579019761409177, ; 59: netstandard.dll => 0x3393f0ed5c8c5c99 => 1
	i64 3727469159507183293, ; 60: Xamarin.AndroidX.RecyclerView => 0x33baa1739ba646bd => 86
	i64 3966267475168208030, ; 61: System.Memory => 0x370b03412596249e => 42
	i64 4202567570116092282, ; 62: Newtonsoft.Json.Bson => 0x3a5284f05958a17a => 26
	i64 4205733251782235008, ; 63: ViewModel.dll => 0x3a5dc41c0c875380 => 3
	i64 4308357023624807347, ; 64: FluentFTP => 0x3bca5be2e6b743b3 => 19
	i64 4337444564132831293, ; 65: SQLitePCLRaw.batteries_v2.dll => 0x3c31b2d9ae16203d => 35
	i64 4507253840557699464, ; 66: ApiModels => 0x3e8cfb7ccef4f188 => 2
	i64 4525561845656915374, ; 67: System.ServiceModel.Internals => 0x3ece06856b710dae => 118
	i64 4636684751163556186, ; 68: Xamarin.AndroidX.VersionedParcelable.dll => 0x4058d0370893015a => 93
	i64 4782108999019072045, ; 69: Xamarin.AndroidX.AsyncLayoutInflater.dll => 0x425d76cc43bb0a2d => 60
	i64 4794310189461587505, ; 70: Xamarin.AndroidX.Activity => 0x4288cfb749e4c631 => 53
	i64 4795410492532947900, ; 71: Xamarin.AndroidX.SwipeRefreshLayout.dll => 0x428cb86f8f9b7bbc => 89
	i64 5142919913060024034, ; 72: Xamarin.Forms.Platform.Android.dll => 0x475f52699e39bee2 => 100
	i64 5202753749449073649, ; 73: Plugin.Media => 0x4833e4f841be63f1 => 30
	i64 5203618020066742981, ; 74: Xamarin.Essentials => 0x4836f704f0e652c5 => 97
	i64 5205316157927637098, ; 75: Xamarin.AndroidX.LocalBroadcastManager => 0x483cff7778e0c06a => 82
	i64 5348796042099802469, ; 76: Xamarin.AndroidX.Media => 0x4a3abda9415fc165 => 83
	i64 5375264076663995773, ; 77: Xamarin.Forms.PancakeView => 0x4a98c632c779bd7d => 99
	i64 5376510917114486089, ; 78: Xamarin.AndroidX.VectorDrawable.Animated => 0x4a9d3431719e5d49 => 91
	i64 5408338804355907810, ; 79: Xamarin.AndroidX.Transition => 0x4b0e477cea9840e2 => 90
	i64 5446034149219586269, ; 80: System.Diagnostics.Debug => 0x4b94333452e150dd => 7
	i64 5507995362134886206, ; 81: System.Core.dll => 0x4c705499688c873e => 40
	i64 5692067934154308417, ; 82: Xamarin.AndroidX.ViewPager2.dll => 0x4efe49a0d4a8bb41 => 95
	i64 5807501971057721934, ; 83: BRBKApp => 0x50986445f146c24e => 17
	i64 5896680224035167651, ; 84: Xamarin.AndroidX.Lifecycle.LiveData.dll => 0x51d5376bfbafdda3 => 77
	i64 6085203216496545422, ; 85: Xamarin.Forms.Platform.dll => 0x5472fc15a9574e8e => 101
	i64 6086316965293125504, ; 86: FormsViewGroup.dll => 0x5476f10882baef80 => 20
	i64 6183170893902868313, ; 87: SQLitePCLRaw.batteries_v2 => 0x55cf092b0c9d6f59 => 35
	i64 6222399776351216807, ; 88: System.Text.Json.dll => 0x565a67a0ffe264a7 => 50
	i64 6319713645133255417, ; 89: Xamarin.AndroidX.Lifecycle.Runtime => 0x57b42213b45b52f9 => 78
	i64 6401687960814735282, ; 90: Xamarin.AndroidX.Lifecycle.LiveData.Core => 0x58d75d486341cfb2 => 76
	i64 6504860066809920875, ; 91: Xamarin.AndroidX.Browser.dll => 0x5a45e7c43bd43d6b => 61
	i64 6548213210057960872, ; 92: Xamarin.AndroidX.CustomView.dll => 0x5adfed387b066da8 => 67
	i64 6591024623626361694, ; 93: System.Web.Services.dll => 0x5b7805f9751a1b5e => 112
	i64 6659513131007730089, ; 94: Xamarin.AndroidX.Legacy.Support.Core.UI.dll => 0x5c6b57e8b6c3e1a9 => 72
	i64 6876862101832370452, ; 95: System.Xml.Linq => 0x5f6f85a57d108914 => 52
	i64 6894844156784520562, ; 96: System.Numerics.Vectors => 0x5faf683aead1ad72 => 45
	i64 7036436454368433159, ; 97: Xamarin.AndroidX.Legacy.Support.V4.dll => 0x61a671acb33d5407 => 74
	i64 7103753931438454322, ; 98: Xamarin.AndroidX.Interpolator.dll => 0x62959a90372c7632 => 71
	i64 7270811800166795866, ; 99: System.Linq => 0x64e71ccf51a90a5a => 116
	i64 7403750374579903384, ; 100: SharpSnmpLib.dll => 0x66bf67bd2a801b98 => 33
	i64 7488575175965059935, ; 101: System.Xml.Linq.dll => 0x67ecc3724534ab5f => 52
	i64 7635363394907363464, ; 102: Xamarin.Forms.Core.dll => 0x69f6428dc4795888 => 98
	i64 7637365915383206639, ; 103: Xamarin.Essentials.dll => 0x69fd5fd5e61792ef => 97
	i64 7654504624184590948, ; 104: System.Net.Http => 0x6a3a4366801b8264 => 13
	i64 7820441508502274321, ; 105: System.Data => 0x6c87ca1e14ff8111 => 106
	i64 7836164640616011524, ; 106: Xamarin.AndroidX.AppCompat.AppCompatResources => 0x6cbfa6390d64d704 => 56
	i64 7875371864198757046, ; 107: AndHUD.dll => 0x6d4af0fc27ac3ab6 => 15
	i64 8044118961405839122, ; 108: System.ComponentModel.Composition => 0x6fa2739369944712 => 111
	i64 8064050204834738623, ; 109: System.Collections.dll => 0x6fe942efa61731bf => 9
	i64 8083354569033831015, ; 110: Xamarin.AndroidX.Lifecycle.Common.dll => 0x702dd82730cad267 => 75
	i64 8103644804370223335, ; 111: System.Data.DataSetExtensions.dll => 0x7075ee03be6d50e7 => 108
	i64 8167236081217502503, ; 112: Java.Interop.dll => 0x7157d9f1a9b8fd27 => 21
	i64 8185542183669246576, ; 113: System.Collections => 0x7198e33f4794aa70 => 9
	i64 8290740647658429042, ; 114: System.Runtime.Extensions => 0x730ea0b15c929a72 => 115
	i64 8500211157240745237, ; 115: ApiDatos => 0x75f6d102ef258d15 => 4
	i64 8601935802264776013, ; 116: Xamarin.AndroidX.Transition.dll => 0x7760370982b4ed4d => 90
	i64 8626175481042262068, ; 117: Java.Interop => 0x77b654e585b55834 => 21
	i64 8684531736582871431, ; 118: System.IO.Compression.FileSystem => 0x7885a79a0fa0d987 => 110
	i64 9041985878101337471, ; 119: BouncyCastle.Crypto => 0x7d7b963fe854257f => 16
	i64 9270958243998599961, ; 120: Csv => 0x80a90f673d040b19 => 18
	i64 9312692141327339315, ; 121: Xamarin.AndroidX.ViewPager2 => 0x813d54296a634f33 => 95
	i64 9324707631942237306, ; 122: Xamarin.AndroidX.AppCompat => 0x8168042fd44a7c7a => 57
	i64 9581837667595819671, ; 123: SdkApi.Core.dll => 0x84f9869788819a97 => 32
	i64 9584643793929893533, ; 124: System.IO.dll => 0x85037ebfbbd7f69d => 113
	i64 9659729154652888475, ; 125: System.Text.RegularExpressions => 0x860e407c9991dd9b => 114
	i64 9662334977499516867, ; 126: System.Numerics.dll => 0x8617827802b0cfc3 => 44
	i64 9678050649315576968, ; 127: Xamarin.AndroidX.CoordinatorLayout.dll => 0x864f57c9feb18c88 => 64
	i64 9711637524876806384, ; 128: Xamarin.AndroidX.Media.dll => 0x86c6aadfd9a2c8f0 => 83
	i64 9780723996889434231, ; 129: AndHUD => 0x87bc1ca798bbc877 => 15
	i64 9808709177481450983, ; 130: Mono.Android.dll => 0x881f890734e555e7 => 24
	i64 9834056768316610435, ; 131: System.Transactions.dll => 0x8879968718899783 => 107
	i64 9994308163963284983, ; 132: Newtonsoft.Json.Bson.dll => 0x8ab2ea52b0d16df7 => 26
	i64 9998632235833408227, ; 133: Mono.Security => 0x8ac2470b209ebae3 => 117
	i64 10038780035334861115, ; 134: System.Net.Http.dll => 0x8b50e941206af13b => 13
	i64 10229024438826829339, ; 135: Xamarin.AndroidX.CustomView => 0x8df4cb880b10061b => 67
	i64 10272178638466820556, ; 136: Csv.dll => 0x8e8e1c0c1f2ba9cc => 18
	i64 10360651442923773544, ; 137: System.Text.Encoding => 0x8fc86d98211c1e68 => 8
	i64 10430153318873392755, ; 138: Xamarin.AndroidX.Core => 0x90bf592ea44f6673 => 65
	i64 10447083246144586668, ; 139: Microsoft.Bcl.AsyncInterfaces.dll => 0x90fb7edc816203ac => 22
	i64 10566960649245365243, ; 140: System.Globalization.dll => 0x92a562b96dcd13fb => 11
	i64 10714184849103829812, ; 141: System.Runtime.Extensions.dll => 0x94b06e5aa4b4bb34 => 115
	i64 10847732767863316357, ; 142: Xamarin.AndroidX.Arch.Core.Common => 0x968ae37a86db9f85 => 58
	i64 10961253253801991749, ; 143: SharpSnmpLib => 0x981e31c255f65e45 => 33
	i64 11023048688141570732, ; 144: System.Core => 0x98f9bc61168392ac => 40
	i64 11037814507248023548, ; 145: System.Xml => 0x992e31d0412bf7fc => 51
	i64 11122995063473561350, ; 146: Xamarin.CommunityToolkit.dll => 0x9a5cd113fcc3df06 => 96
	i64 11162124722117608902, ; 147: Xamarin.AndroidX.ViewPager => 0x9ae7d54b986d05c6 => 94
	i64 11340910727871153756, ; 148: Xamarin.AndroidX.CursorAdapter => 0x9d630238642d465c => 66
	i64 11359010601859007766, ; 149: ZebraPrinterSdk.dll => 0x9da34ff5b3b3c516 => 105
	i64 11392833485892708388, ; 150: Xamarin.AndroidX.Print.dll => 0x9e1b79b18fcf6824 => 85
	i64 11529969570048099689, ; 151: Xamarin.AndroidX.ViewPager.dll => 0xa002ae3c4dc7c569 => 94
	i64 11578238080964724296, ; 152: Xamarin.AndroidX.Legacy.Support.V4 => 0xa0ae2a30c4cd8648 => 74
	i64 11580057168383206117, ; 153: Xamarin.AndroidX.Annotation => 0xa0b4a0a4103262e5 => 54
	i64 11597940890313164233, ; 154: netstandard => 0xa0f429ca8d1805c9 => 1
	i64 11672361001936329215, ; 155: Xamarin.AndroidX.Interpolator => 0xa1fc8e7d0a8999ff => 71
	i64 11739066727115742305, ; 156: SQLite-net.dll => 0xa2e98afdf8575c61 => 34
	i64 11743665907891708234, ; 157: System.Threading.Tasks => 0xa2f9e1ec30c0214a => 6
	i64 11806260347154423189, ; 158: SQLite-net => 0xa3d8433bc5eb5d95 => 34
	i64 12102847907131387746, ; 159: System.Buffers => 0xa7f5f40c43256f62 => 39
	i64 12137774235383566651, ; 160: Xamarin.AndroidX.VectorDrawable => 0xa872095bbfed113b => 92
	i64 12145600677423100781, ; 161: ViewModel => 0xa88dd777498c336d => 3
	i64 12145679461940342714, ; 162: System.Text.Json => 0xa88e1f1ebcb62fba => 50
	i64 12193402982461711035, ; 163: BRBKApp.Android.dll => 0xa937ab68bcd1a2bb => 0
	i64 12279246230491828964, ; 164: SQLitePCLRaw.provider.e_sqlite3.dll => 0xaa68a5636e0512e4 => 38
	i64 12451044538927396471, ; 165: Xamarin.AndroidX.Fragment.dll => 0xaccaff0a2955b677 => 70
	i64 12466513435562512481, ; 166: Xamarin.AndroidX.Loader.dll => 0xad01f3eb52569061 => 81
	i64 12487638416075308985, ; 167: Xamarin.AndroidX.DocumentFile.dll => 0xad4d00fa21b0bfb9 => 68
	i64 12501328358063843848, ; 168: Plugin.Geolocator.dll => 0xad7da3e822e9aa08 => 29
	i64 12538491095302438457, ; 169: Xamarin.AndroidX.CardView.dll => 0xae01ab382ae67e39 => 62
	i64 12550732019250633519, ; 170: System.IO.Compression => 0xae2d28465e8e1b2f => 109
	i64 12700543734426720211, ; 171: Xamarin.AndroidX.Collection => 0xb041653c70d157d3 => 63
	i64 12708238894395270091, ; 172: System.IO => 0xb05cbbf17d3ba3cb => 113
	i64 12708922737231849740, ; 173: System.Text.Encoding.Extensions => 0xb05f29e50e96e90c => 12
	i64 12963446364377008305, ; 174: System.Drawing.Common.dll => 0xb3e769c8fd8548b1 => 5
	i64 13081516019875753442, ; 175: BouncyCastle.Crypto.dll => 0xb58ae182e046ade2 => 16
	i64 13174891378228215420, ; 176: BRBKApp.Android => 0xb6d69de7a9a63e7c => 0
	i64 13370592475155966277, ; 177: System.Runtime.Serialization => 0xb98de304062ea945 => 48
	i64 13401370062847626945, ; 178: Xamarin.AndroidX.VectorDrawable.dll => 0xb9fb3b1193964ec1 => 92
	i64 13454009404024712428, ; 179: Xamarin.Google.Guava.ListenableFuture => 0xbab63e4543a86cec => 104
	i64 13491513212026656886, ; 180: Xamarin.AndroidX.Arch.Core.Runtime.dll => 0xbb3b7bc905569876 => 59
	i64 13572454107664307259, ; 181: Xamarin.AndroidX.RecyclerView.dll => 0xbc5b0b19d99f543b => 86
	i64 13643785327914841093, ; 182: Plugin.Media.dll => 0xbd587677c60cf405 => 30
	i64 13647894001087880694, ; 183: System.Data.dll => 0xbd670f48cb071df6 => 106
	i64 13955418299340266673, ; 184: Microsoft.Extensions.DependencyModel.dll => 0xc1ab9b0118299cb1 => 23
	i64 13959074834287824816, ; 185: Xamarin.AndroidX.Fragment => 0xc1b8989a7ad20fb0 => 70
	i64 13967638549803255703, ; 186: Xamarin.Forms.Platform.Android => 0xc1d70541e0134797 => 100
	i64 14124974489674258913, ; 187: Xamarin.AndroidX.CardView => 0xc405fd76067d19e1 => 62
	i64 14161076099266624234, ; 188: Acr.UserDialogs => 0xc4863faf060fbaea => 14
	i64 14172845254133543601, ; 189: Xamarin.AndroidX.MultiDex => 0xc4b00faaed35f2b1 => 84
	i64 14254574811015963973, ; 190: System.Text.Encoding.Extensions.dll => 0xc5d26c4442d66545 => 12
	i64 14261073672896646636, ; 191: Xamarin.AndroidX.Print => 0xc5e982f274ae0dec => 85
	i64 14296230332995809285, ; 192: BRBKApp.dll => 0xc66669be41613005 => 17
	i64 14486659737292545672, ; 193: Xamarin.AndroidX.Lifecycle.LiveData => 0xc90af44707469e88 => 77
	i64 14551742072151931844, ; 194: System.Text.Encodings.Web.dll => 0xc9f22c50f1b8fbc4 => 49
	i64 14644440854989303794, ; 195: Xamarin.AndroidX.LocalBroadcastManager.dll => 0xcb3b815e37daeff2 => 82
	i64 14792063746108907174, ; 196: Xamarin.Google.Guava.ListenableFuture.dll => 0xcd47f79af9c15ea6 => 104
	i64 14852515768018889994, ; 197: Xamarin.AndroidX.CursorAdapter.dll => 0xce1ebc6625a76d0a => 66
	i64 14987728460634540364, ; 198: System.IO.Compression.dll => 0xcfff1ba06622494c => 109
	i64 14988210264188246988, ; 199: Xamarin.AndroidX.DocumentFile => 0xd000d1d307cddbcc => 68
	i64 15133485256822086103, ; 200: System.Linq.dll => 0xd204f0a9127dd9d7 => 116
	i64 15259450923142030757, ; 201: ZebraPrinterSdk => 0xd3c475c43d6325a5 => 105
	i64 15370334346939861994, ; 202: Xamarin.AndroidX.Core.dll => 0xd54e65a72c560bea => 65
	i64 15526743539506359484, ; 203: System.Text.Encoding.dll => 0xd77a12fc26de2cbc => 8
	i64 15582737692548360875, ; 204: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 0xd841015ed86f6aab => 80
	i64 15609085926864131306, ; 205: System.dll => 0xd89e9cf3334914ea => 41
	i64 15620595871140898079, ; 206: Microsoft.Extensions.DependencyModel => 0xd8c7812eef49651f => 23
	i64 15777549416145007739, ; 207: Xamarin.AndroidX.SlidingPaneLayout.dll => 0xdaf51d99d77eb47b => 88
	i64 15810740023422282496, ; 208: Xamarin.Forms.Xaml => 0xdb6b08484c22eb00 => 102
	i64 15817206913877585035, ; 209: System.Threading.Tasks.dll => 0xdb8201e29086ac8b => 6
	i64 15962433837198553176, ; 210: ApiDatos.dll => 0xdd85f50098e53058 => 4
	i64 15963349826457351533, ; 211: System.Threading.Tasks.Extensions => 0xdd893616f748b56d => 119
	i64 16154507427712707110, ; 212: System => 0xe03056ea4e39aa26 => 41
	i64 16427145038571576407, ; 213: FluentFTP.dll => 0xe3f8f160b9e11c57 => 19
	i64 16565028646146589191, ; 214: System.ComponentModel.Composition.dll => 0xe5e2cdc9d3bcc207 => 111
	i64 16755018182064898362, ; 215: SQLitePCLRaw.core.dll => 0xe885c843c330813a => 36
	i64 16816077459021405854, ; 216: ApiModels.dll => 0xe95eb55a846c9e9e => 2
	i64 16822611501064131242, ; 217: System.Data.DataSetExtensions => 0xe975ec07bb5412aa => 108
	i64 16833383113903931215, ; 218: mscorlib => 0xe99c30c1484d7f4f => 25
	i64 16890310621557459193, ; 219: System.Text.RegularExpressions.dll => 0xea66700587f088f9 => 114
	i64 16895806301542741427, ; 220: Plugin.Permissions => 0xea79f6503d42f5b3 => 31
	i64 17024911836938395553, ; 221: Xamarin.AndroidX.Annotation.Experimental.dll => 0xec44a31d250e5fa1 => 55
	i64 17037200463775726619, ; 222: Xamarin.AndroidX.Legacy.Support.Core.Utils => 0xec704b8e0a78fc1b => 73
	i64 17544493274320527064, ; 223: Xamarin.AndroidX.AsyncLayoutInflater => 0xf37a8fada41aded8 => 60
	i64 17627500474728259406, ; 224: System.Globalization => 0xf4a176498a351f4e => 11
	i64 17685921127322830888, ; 225: System.Diagnostics.Debug.dll => 0xf571038fafa74828 => 7
	i64 17704177640604968747, ; 226: Xamarin.AndroidX.Loader => 0xf5b1dfc36cac272b => 81
	i64 17710060891934109755, ; 227: Xamarin.AndroidX.Lifecycle.ViewModel => 0xf5c6c68c9e45303b => 79
	i64 17727188866493996799, ; 228: System.Net.Http.Formatting.dll => 0xf603a059f5a25eff => 43
	i64 17777860260071588075, ; 229: System.Runtime.Numerics.dll => 0xf6b7a5b72419c0eb => 10
	i64 17786996386789405829, ; 230: Plugin.Geolocator => 0xf6d81af967bd3485 => 29
	i64 17827832363535584534, ; 231: Xamarin.Forms.PancakeView.dll => 0xf7692f1427c04d16 => 99
	i64 17838668724098252521, ; 232: System.Buffers.dll => 0xf78faeb0f5bf3ee9 => 39
	i64 17882897186074144999, ; 233: FormsViewGroup => 0xf82cd03e3ac830e7 => 20
	i64 17892495832318972303, ; 234: Xamarin.Forms.Xaml.dll => 0xf84eea293687918f => 102
	i64 17928294245072900555, ; 235: System.IO.Compression.FileSystem.dll => 0xf8ce18a0b24011cb => 110
	i64 18116111925905154859, ; 236: Xamarin.AndroidX.Arch.Core.Runtime => 0xfb695bd036cb632b => 59
	i64 18129453464017766560, ; 237: System.ServiceModel.Internals.dll => 0xfb98c1df1ec108a0 => 118
	i64 18370042311372477656, ; 238: SQLitePCLRaw.lib.e_sqlite3.android.dll => 0xfeef80274e4094d8 => 37
	i64 18380184030268848184 ; 239: Xamarin.AndroidX.VersionedParcelable => 0xff1387fe3e7b7838 => 93
], align 8
@assembly_image_cache_indices = local_unnamed_addr constant [240 x i32] [
	i32 24, i32 43, i32 63, i32 87, i32 88, i32 78, i32 32, i32 5, ; 0..7
	i32 72, i32 37, i32 69, i32 107, i32 101, i32 117, i32 103, i32 36, ; 8..15
	i32 58, i32 31, i32 48, i32 38, i32 56, i32 80, i32 73, i32 27, ; 16..23
	i32 42, i32 57, i32 87, i32 22, i32 54, i32 79, i32 96, i32 119, ; 24..31
	i32 27, i32 84, i32 61, i32 69, i32 112, i32 76, i32 49, i32 46, ; 32..39
	i32 64, i32 91, i32 14, i32 47, i32 53, i32 51, i32 25, i32 46, ; 40..47
	i32 28, i32 98, i32 103, i32 75, i32 55, i32 45, i32 10, i32 89, ; 48..55
	i32 44, i32 47, i32 28, i32 1, i32 86, i32 42, i32 26, i32 3, ; 56..63
	i32 19, i32 35, i32 2, i32 118, i32 93, i32 60, i32 53, i32 89, ; 64..71
	i32 100, i32 30, i32 97, i32 82, i32 83, i32 99, i32 91, i32 90, ; 72..79
	i32 7, i32 40, i32 95, i32 17, i32 77, i32 101, i32 20, i32 35, ; 80..87
	i32 50, i32 78, i32 76, i32 61, i32 67, i32 112, i32 72, i32 52, ; 88..95
	i32 45, i32 74, i32 71, i32 116, i32 33, i32 52, i32 98, i32 97, ; 96..103
	i32 13, i32 106, i32 56, i32 15, i32 111, i32 9, i32 75, i32 108, ; 104..111
	i32 21, i32 9, i32 115, i32 4, i32 90, i32 21, i32 110, i32 16, ; 112..119
	i32 18, i32 95, i32 57, i32 32, i32 113, i32 114, i32 44, i32 64, ; 120..127
	i32 83, i32 15, i32 24, i32 107, i32 26, i32 117, i32 13, i32 67, ; 128..135
	i32 18, i32 8, i32 65, i32 22, i32 11, i32 115, i32 58, i32 33, ; 136..143
	i32 40, i32 51, i32 96, i32 94, i32 66, i32 105, i32 85, i32 94, ; 144..151
	i32 74, i32 54, i32 1, i32 71, i32 34, i32 6, i32 34, i32 39, ; 152..159
	i32 92, i32 3, i32 50, i32 0, i32 38, i32 70, i32 81, i32 68, ; 160..167
	i32 29, i32 62, i32 109, i32 63, i32 113, i32 12, i32 5, i32 16, ; 168..175
	i32 0, i32 48, i32 92, i32 104, i32 59, i32 86, i32 30, i32 106, ; 176..183
	i32 23, i32 70, i32 100, i32 62, i32 14, i32 84, i32 12, i32 85, ; 184..191
	i32 17, i32 77, i32 49, i32 82, i32 104, i32 66, i32 109, i32 68, ; 192..199
	i32 116, i32 105, i32 65, i32 8, i32 80, i32 41, i32 23, i32 88, ; 200..207
	i32 102, i32 6, i32 4, i32 119, i32 41, i32 19, i32 111, i32 36, ; 208..215
	i32 2, i32 108, i32 25, i32 114, i32 31, i32 55, i32 73, i32 60, ; 216..223
	i32 11, i32 7, i32 81, i32 79, i32 43, i32 10, i32 29, i32 99, ; 224..231
	i32 39, i32 20, i32 102, i32 110, i32 59, i32 118, i32 37, i32 93 ; 240..239
], align 4

@marshal_methods_number_of_classes = local_unnamed_addr constant i32 0, align 4

; marshal_methods_class_cache
@marshal_methods_class_cache = global [0 x %struct.MarshalMethodsManagedClass] [
], align 8; end of 'marshal_methods_class_cache' array


@get_function_pointer = internal unnamed_addr global void (i32, i32, i32, i8**)* null, align 8

; Function attributes: "frame-pointer"="non-leaf" "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+neon,+outline-atomics" uwtable willreturn writeonly
define void @xamarin_app_init (void (i32, i32, i32, i8**)* %fn) local_unnamed_addr #0
{
	store void (i32, i32, i32, i8**)* %fn, void (i32, i32, i32, i8**)** @get_function_pointer, align 8
	ret void
}

; Names of classes in which marshal methods reside
@mm_class_names = local_unnamed_addr constant [0 x i8*] zeroinitializer, align 8
@__MarshalMethodName_name.0 = internal constant [1 x i8] c"\00", align 1

; mm_method_names
@mm_method_names = local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	; 0
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		i8* getelementptr inbounds ([1 x i8], [1 x i8]* @__MarshalMethodName_name.0, i32 0, i32 0); name
	}
], align 8; end of 'mm_method_names' array


attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable willreturn writeonly "frame-pointer"="non-leaf" "target-cpu"="generic" "target-features"="+neon,+outline-atomics" }
attributes #1 = { "min-legal-vector-width"="0" mustprogress "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable "frame-pointer"="non-leaf" "target-cpu"="generic" "target-features"="+neon,+outline-atomics" }
attributes #2 = { nounwind }

!llvm.module.flags = !{!0, !1, !2, !3, !4, !5}
!llvm.ident = !{!6}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!2 = !{i32 1, !"branch-target-enforcement", i32 0}
!3 = !{i32 1, !"sign-return-address", i32 0}
!4 = !{i32 1, !"sign-return-address-all", i32 0}
!5 = !{i32 1, !"sign-return-address-with-bkey", i32 0}
!6 = !{!"Xamarin.Android remotes/origin/d17-5 @ 45b0e144f73b2c8747d8b5ec8cbd3b55beca67f0"}
!llvm.linker.options = !{}
