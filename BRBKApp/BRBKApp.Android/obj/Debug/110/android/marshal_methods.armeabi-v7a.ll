; ModuleID = 'obj\Debug\110\android\marshal_methods.armeabi-v7a.ll'
source_filename = "obj\Debug\110\android\marshal_methods.armeabi-v7a.ll"
target datalayout = "e-m:e-p:32:32-Fi8-i64:64-v128:64:128-a:0:32-n32-S64"
target triple = "armv7-unknown-linux-android"


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
@assembly_image_cache = local_unnamed_addr global [0 x %struct.MonoImage*] zeroinitializer, align 4
; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = local_unnamed_addr constant [240 x i32] [
	i32 26230656, ; 0: Microsoft.Extensions.DependencyModel => 0x1903f80 => 23
	i32 32687329, ; 1: Xamarin.AndroidX.Lifecycle.Runtime => 0x1f2c4e1 => 78
	i32 34715100, ; 2: Xamarin.Google.Guava.ListenableFuture.dll => 0x211b5dc => 104
	i32 39109920, ; 3: Newtonsoft.Json.dll => 0x254c520 => 27
	i32 39852199, ; 4: Plugin.Permissions => 0x26018a7 => 31
	i32 57263871, ; 5: Xamarin.Forms.Core.dll => 0x369c6ff => 98
	i32 88799905, ; 6: Acr.UserDialogs => 0x54afaa1 => 14
	i32 101534019, ; 7: Xamarin.AndroidX.SlidingPaneLayout => 0x60d4943 => 88
	i32 120558881, ; 8: Xamarin.AndroidX.SlidingPaneLayout.dll => 0x72f9521 => 88
	i32 165246403, ; 9: Xamarin.AndroidX.Collection.dll => 0x9d975c3 => 63
	i32 182336117, ; 10: Xamarin.AndroidX.SwipeRefreshLayout.dll => 0xade3a75 => 89
	i32 209399409, ; 11: Xamarin.AndroidX.Browser.dll => 0xc7b2e71 => 61
	i32 220171995, ; 12: System.Diagnostics.Debug => 0xd1f8edb => 7
	i32 230216969, ; 13: Xamarin.AndroidX.Legacy.Support.Core.Utils.dll => 0xdb8d509 => 73
	i32 231814094, ; 14: System.Globalization => 0xdd133ce => 11
	i32 232815796, ; 15: System.Web.Services => 0xde07cb4 => 112
	i32 278686392, ; 16: Xamarin.AndroidX.Lifecycle.LiveData.dll => 0x109c6ab8 => 77
	i32 280482487, ; 17: Xamarin.AndroidX.Interpolator => 0x10b7d2b7 => 71
	i32 292434203, ; 18: Csv => 0x116e311b => 18
	i32 318968648, ; 19: Xamarin.AndroidX.Activity.dll => 0x13031348 => 53
	i32 321597661, ; 20: System.Numerics => 0x132b30dd => 44
	i32 342366114, ; 21: Xamarin.AndroidX.Lifecycle.Common => 0x146817a2 => 75
	i32 347068432, ; 22: SQLitePCLRaw.lib.e_sqlite3.android.dll => 0x14afd810 => 37
	i32 385762202, ; 23: System.Memory.dll => 0x16fe439a => 42
	i32 442521989, ; 24: Xamarin.Essentials => 0x1a605985 => 97
	i32 442565967, ; 25: System.Collections => 0x1a61054f => 9
	i32 450948140, ; 26: Xamarin.AndroidX.Fragment.dll => 0x1ae0ec2c => 70
	i32 465846621, ; 27: mscorlib => 0x1bc4415d => 25
	i32 469710990, ; 28: System.dll => 0x1bff388e => 41
	i32 476646585, ; 29: Xamarin.AndroidX.Interpolator.dll => 0x1c690cb9 => 71
	i32 486930444, ; 30: Xamarin.AndroidX.LocalBroadcastManager.dll => 0x1d05f80c => 82
	i32 526420162, ; 31: System.Transactions.dll => 0x1f6088c2 => 107
	i32 545304856, ; 32: System.Runtime.Extensions => 0x2080b118 => 115
	i32 548916678, ; 33: Microsoft.Bcl.AsyncInterfaces => 0x20b7cdc6 => 22
	i32 605376203, ; 34: System.IO.Compression.FileSystem => 0x24154ecb => 110
	i32 627609679, ; 35: Xamarin.AndroidX.CustomView => 0x2568904f => 67
	i32 662205335, ; 36: System.Text.Encodings.Web.dll => 0x27787397 => 49
	i32 663517072, ; 37: Xamarin.AndroidX.VersionedParcelable => 0x278c7790 => 93
	i32 666292255, ; 38: Xamarin.AndroidX.Arch.Core.Common.dll => 0x27b6d01f => 58
	i32 682508585, ; 39: BRBKApp.Android.dll => 0x28ae4129 => 0
	i32 690569205, ; 40: System.Xml.Linq.dll => 0x29293ff5 => 52
	i32 691439157, ; 41: Acr.UserDialogs.dll => 0x29368635 => 14
	i32 748832960, ; 42: SQLitePCLRaw.batteries_v2 => 0x2ca248c0 => 35
	i32 762297302, ; 43: SdkApi.Core.dll => 0x2d6fbbd6 => 32
	i32 775507847, ; 44: System.IO.Compression => 0x2e394f87 => 109
	i32 809851609, ; 45: System.Drawing.Common.dll => 0x30455ad9 => 5
	i32 843511501, ; 46: Xamarin.AndroidX.Print => 0x3246f6cd => 85
	i32 866389463, ; 47: ViewModel => 0x33a40dd7 => 3
	i32 877678880, ; 48: System.Globalization.dll => 0x34505120 => 11
	i32 924627022, ; 49: SharpSnmpLib => 0x371cb04e => 33
	i32 928116545, ; 50: Xamarin.Google.Guava.ListenableFuture => 0x3751ef41 => 104
	i32 955402788, ; 51: Newtonsoft.Json => 0x38f24a24 => 27
	i32 957807352, ; 52: Plugin.CurrentActivity => 0x3916faf8 => 28
	i32 967690846, ; 53: Xamarin.AndroidX.Lifecycle.Common.dll => 0x39adca5e => 75
	i32 974778368, ; 54: FormsViewGroup.dll => 0x3a19f000 => 20
	i32 992768348, ; 55: System.Collections.dll => 0x3b2c715c => 9
	i32 1012816738, ; 56: Xamarin.AndroidX.SavedState.dll => 0x3c5e5b62 => 87
	i32 1035644815, ; 57: Xamarin.AndroidX.AppCompat => 0x3dbaaf8f => 57
	i32 1042160112, ; 58: Xamarin.Forms.Platform.dll => 0x3e1e19f0 => 101
	i32 1052210849, ; 59: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 0x3eb776a1 => 79
	i32 1098259244, ; 60: System => 0x41761b2c => 41
	i32 1104002344, ; 61: Plugin.Media => 0x41cdbd28 => 30
	i32 1137654822, ; 62: Plugin.Permissions.dll => 0x43cf3c26 => 31
	i32 1175144683, ; 63: Xamarin.AndroidX.VectorDrawable.Animated => 0x460b48eb => 91
	i32 1186231468, ; 64: Newtonsoft.Json.Bson.dll => 0x46b474ac => 26
	i32 1204270330, ; 65: Xamarin.AndroidX.Arch.Core.Common => 0x47c7b4fa => 58
	i32 1267360935, ; 66: Xamarin.AndroidX.VectorDrawable => 0x4b8a64a7 => 92
	i32 1269851834, ; 67: BouncyCastle.Crypto => 0x4bb066ba => 16
	i32 1282958517, ; 68: Plugin.Geolocator => 0x4c7864b5 => 29
	i32 1292207520, ; 69: SQLitePCLRaw.core.dll => 0x4d0585a0 => 36
	i32 1293217323, ; 70: Xamarin.AndroidX.DrawerLayout.dll => 0x4d14ee2b => 69
	i32 1324164729, ; 71: System.Linq => 0x4eed2679 => 116
	i32 1364015309, ; 72: System.IO => 0x514d38cd => 113
	i32 1365406463, ; 73: System.ServiceModel.Internals.dll => 0x516272ff => 118
	i32 1376866003, ; 74: Xamarin.AndroidX.SavedState => 0x52114ed3 => 87
	i32 1395857551, ; 75: Xamarin.AndroidX.Media.dll => 0x5333188f => 83
	i32 1406073936, ; 76: Xamarin.AndroidX.CoordinatorLayout => 0x53cefc50 => 64
	i32 1411638395, ; 77: System.Runtime.CompilerServices.Unsafe => 0x5423e47b => 46
	i32 1457743152, ; 78: System.Runtime.Extensions.dll => 0x56e36530 => 115
	i32 1460219004, ; 79: Xamarin.Forms.Xaml => 0x57092c7c => 102
	i32 1462112819, ; 80: System.IO.Compression.dll => 0x57261233 => 109
	i32 1469204771, ; 81: Xamarin.AndroidX.AppCompat.AppCompatResources => 0x57924923 => 56
	i32 1543031311, ; 82: System.Text.RegularExpressions.dll => 0x5bf8ca0f => 114
	i32 1582372066, ; 83: Xamarin.AndroidX.DocumentFile.dll => 0x5e5114e2 => 68
	i32 1592978981, ; 84: System.Runtime.Serialization.dll => 0x5ef2ee25 => 48
	i32 1596753216, ; 85: BouncyCastle.Crypto.dll => 0x5f2c8540 => 16
	i32 1622152042, ; 86: Xamarin.AndroidX.Loader.dll => 0x60b0136a => 81
	i32 1624863272, ; 87: Xamarin.AndroidX.ViewPager2 => 0x60d97228 => 95
	i32 1636350590, ; 88: Xamarin.AndroidX.CursorAdapter => 0x6188ba7e => 66
	i32 1639515021, ; 89: System.Net.Http.dll => 0x61b9038d => 13
	i32 1639986890, ; 90: System.Text.RegularExpressions => 0x61c036ca => 114
	i32 1649968348, ; 91: BRBKApp.Android => 0x625884dc => 0
	i32 1657153582, ; 92: System.Runtime => 0x62c6282e => 47
	i32 1658251792, ; 93: Xamarin.Google.Android.Material.dll => 0x62d6ea10 => 103
	i32 1701541528, ; 94: System.Diagnostics.Debug.dll => 0x656b7698 => 7
	i32 1711441057, ; 95: SQLitePCLRaw.lib.e_sqlite3.android => 0x660284a1 => 37
	i32 1726428555, ; 96: ApiDatos => 0x66e7358b => 4
	i32 1729485958, ; 97: Xamarin.AndroidX.CardView.dll => 0x6715dc86 => 62
	i32 1766324549, ; 98: Xamarin.AndroidX.SwipeRefreshLayout => 0x6947f945 => 89
	i32 1776026572, ; 99: System.Core.dll => 0x69dc03cc => 40
	i32 1788241197, ; 100: Xamarin.AndroidX.Fragment => 0x6a96652d => 70
	i32 1796167890, ; 101: Microsoft.Bcl.AsyncInterfaces.dll => 0x6b0f58d2 => 22
	i32 1808609942, ; 102: Xamarin.AndroidX.Loader => 0x6bcd3296 => 81
	i32 1813201214, ; 103: Xamarin.Google.Android.Material => 0x6c13413e => 103
	i32 1824175904, ; 104: System.Text.Encoding.Extensions => 0x6cbab720 => 12
	i32 1867746548, ; 105: Xamarin.Essentials.dll => 0x6f538cf4 => 97
	i32 1878053835, ; 106: Xamarin.Forms.Xaml.dll => 0x6ff0d3cb => 102
	i32 1885316902, ; 107: Xamarin.AndroidX.Arch.Core.Runtime.dll => 0x705fa726 => 59
	i32 1919157823, ; 108: Xamarin.AndroidX.MultiDex.dll => 0x7264063f => 84
	i32 2011961780, ; 109: System.Buffers.dll => 0x77ec19b4 => 39
	i32 2019465201, ; 110: Xamarin.AndroidX.Lifecycle.ViewModel => 0x785e97f1 => 79
	i32 2048185678, ; 111: Plugin.Media.dll => 0x7a14d54e => 30
	i32 2055257422, ; 112: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 0x7a80bd4e => 76
	i32 2069514766, ; 113: Newtonsoft.Json.Bson => 0x7b5a4a0e => 26
	i32 2079903147, ; 114: System.Runtime.dll => 0x7bf8cdab => 47
	i32 2090596640, ; 115: System.Numerics.Vectors => 0x7c9bf920 => 45
	i32 2097448633, ; 116: Xamarin.AndroidX.Legacy.Support.Core.UI => 0x7d0486b9 => 72
	i32 2103459038, ; 117: SQLitePCLRaw.provider.e_sqlite3.dll => 0x7d603cde => 38
	i32 2113902067, ; 118: Xamarin.Forms.PancakeView.dll => 0x7dff95f3 => 99
	i32 2126786730, ; 119: Xamarin.Forms.Platform.Android => 0x7ec430aa => 100
	i32 2197979891, ; 120: Microsoft.Extensions.DependencyModel.dll => 0x830282f3 => 23
	i32 2201231467, ; 121: System.Net.Http => 0x8334206b => 13
	i32 2217644978, ; 122: Xamarin.AndroidX.VectorDrawable.Animated.dll => 0x842e93b2 => 91
	i32 2244775296, ; 123: Xamarin.AndroidX.LocalBroadcastManager => 0x85cc8d80 => 82
	i32 2256548716, ; 124: Xamarin.AndroidX.MultiDex => 0x8680336c => 84
	i32 2261435625, ; 125: Xamarin.AndroidX.Legacy.Support.V4.dll => 0x86cac4e9 => 74
	i32 2279755925, ; 126: Xamarin.AndroidX.RecyclerView.dll => 0x87e25095 => 86
	i32 2287334999, ; 127: ApiModels.dll => 0x8855f657 => 2
	i32 2315684594, ; 128: Xamarin.AndroidX.Annotation.dll => 0x8a068af2 => 54
	i32 2392818924, ; 129: System.Net.Http.Formatting.dll => 0x8e9f84ec => 43
	i32 2397082276, ; 130: Xamarin.Forms.PancakeView => 0x8ee092a4 => 99
	i32 2454642406, ; 131: System.Text.Encoding.dll => 0x924edee6 => 8
	i32 2465273461, ; 132: SQLitePCLRaw.batteries_v2.dll => 0x92f11675 => 35
	i32 2471841756, ; 133: netstandard.dll => 0x93554fdc => 1
	i32 2475788418, ; 134: Java.Interop.dll => 0x93918882 => 21
	i32 2486587502, ; 135: BRBKApp => 0x9436506e => 17
	i32 2501346920, ; 136: System.Data.DataSetExtensions => 0x95178668 => 108
	i32 2505896520, ; 137: Xamarin.AndroidX.Lifecycle.Runtime.dll => 0x955cf248 => 78
	i32 2563143864, ; 138: AndHUD.dll => 0x98c678b8 => 15
	i32 2570120770, ; 139: System.Text.Encodings.Web => 0x9930ee42 => 49
	i32 2581819634, ; 140: Xamarin.AndroidX.VectorDrawable.dll => 0x99e370f2 => 92
	i32 2585220780, ; 141: System.Text.Encoding.Extensions.dll => 0x9a1756ac => 12
	i32 2595235902, ; 142: BRBKApp.dll => 0x9ab0283e => 17
	i32 2620871830, ; 143: Xamarin.AndroidX.CursorAdapter.dll => 0x9c375496 => 66
	i32 2633051222, ; 144: Xamarin.AndroidX.Lifecycle.LiveData => 0x9cf12c56 => 77
	i32 2693849962, ; 145: System.IO.dll => 0xa090e36a => 113
	i32 2715334215, ; 146: System.Threading.Tasks.dll => 0xa1d8b647 => 6
	i32 2724373263, ; 147: System.Runtime.Numerics.dll => 0xa262a30f => 10
	i32 2732626843, ; 148: Xamarin.AndroidX.Activity => 0xa2e0939b => 53
	i32 2737747696, ; 149: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 0xa32eb6f0 => 56
	i32 2766581644, ; 150: Xamarin.Forms.Core => 0xa4e6af8c => 98
	i32 2778768386, ; 151: Xamarin.AndroidX.ViewPager.dll => 0xa5a0a402 => 94
	i32 2806986428, ; 152: Plugin.CurrentActivity.dll => 0xa74f36bc => 28
	i32 2810250172, ; 153: Xamarin.AndroidX.CoordinatorLayout.dll => 0xa78103bc => 64
	i32 2819470561, ; 154: System.Xml.dll => 0xa80db4e1 => 51
	i32 2853208004, ; 155: Xamarin.AndroidX.ViewPager => 0xaa107fc4 => 94
	i32 2855708567, ; 156: Xamarin.AndroidX.Transition => 0xaa36a797 => 90
	i32 2903344695, ; 157: System.ComponentModel.Composition => 0xad0d8637 => 111
	i32 2905242038, ; 158: mscorlib.dll => 0xad2a79b6 => 25
	i32 2916838712, ; 159: Xamarin.AndroidX.ViewPager2.dll => 0xaddb6d38 => 95
	i32 2919462931, ; 160: System.Numerics.Vectors.dll => 0xae037813 => 45
	i32 2921128767, ; 161: Xamarin.AndroidX.Annotation.Experimental.dll => 0xae1ce33f => 55
	i32 2947987946, ; 162: FluentFTP.dll => 0xafb6b9ea => 19
	i32 2978675010, ; 163: Xamarin.AndroidX.DrawerLayout => 0xb18af942 => 69
	i32 2979101390, ; 164: FluentFTP => 0xb1917ace => 19
	i32 3024354802, ; 165: Xamarin.AndroidX.Legacy.Support.Core.Utils => 0xb443fdf2 => 73
	i32 3044182254, ; 166: FormsViewGroup => 0xb57288ee => 20
	i32 3075834255, ; 167: System.Threading.Tasks => 0xb755818f => 6
	i32 3111772706, ; 168: System.Runtime.Serialization => 0xb979e222 => 48
	i32 3124832203, ; 169: System.Threading.Tasks.Extensions => 0xba4127cb => 119
	i32 3126016514, ; 170: Plugin.Geolocator.dll => 0xba533a02 => 29
	i32 3204380047, ; 171: System.Data.dll => 0xbefef58f => 106
	i32 3211777861, ; 172: Xamarin.AndroidX.DocumentFile => 0xbf6fd745 => 68
	i32 3228669708, ; 173: ApiModels => 0xc071970c => 2
	i32 3247949154, ; 174: Mono.Security => 0xc197c562 => 117
	i32 3258312781, ; 175: Xamarin.AndroidX.CardView => 0xc235e84d => 62
	i32 3265893370, ; 176: System.Threading.Tasks.Extensions.dll => 0xc2a993fa => 119
	i32 3267021929, ; 177: Xamarin.AndroidX.AsyncLayoutInflater => 0xc2bacc69 => 60
	i32 3279525732, ; 178: ZebraPrinterSdk.dll => 0xc3799764 => 105
	i32 3286872994, ; 179: SQLite-net.dll => 0xc3e9b3a2 => 34
	i32 3299363146, ; 180: System.Text.Encoding => 0xc4a8494a => 8
	i32 3317135071, ; 181: Xamarin.AndroidX.CustomView.dll => 0xc5b776df => 67
	i32 3317144872, ; 182: System.Data => 0xc5b79d28 => 106
	i32 3340431453, ; 183: Xamarin.AndroidX.Arch.Core.Runtime => 0xc71af05d => 59
	i32 3353484488, ; 184: Xamarin.AndroidX.Legacy.Support.Core.UI.dll => 0xc7e21cc8 => 72
	i32 3353544232, ; 185: Xamarin.CommunityToolkit.dll => 0xc7e30628 => 96
	i32 3358260929, ; 186: System.Text.Json => 0xc82afec1 => 50
	i32 3360279109, ; 187: SQLitePCLRaw.core => 0xc849ca45 => 36
	i32 3362522851, ; 188: Xamarin.AndroidX.Core => 0xc86c06e3 => 65
	i32 3366347497, ; 189: Java.Interop => 0xc8a662e9 => 21
	i32 3374999561, ; 190: Xamarin.AndroidX.RecyclerView => 0xc92a6809 => 86
	i32 3395150330, ; 191: System.Runtime.CompilerServices.Unsafe.dll => 0xca5de1fa => 46
	i32 3404865022, ; 192: System.ServiceModel.Internals => 0xcaf21dfe => 118
	i32 3407215217, ; 193: Xamarin.CommunityToolkit => 0xcb15fa71 => 96
	i32 3429136800, ; 194: System.Xml => 0xcc6479a0 => 51
	i32 3430777524, ; 195: netstandard => 0xcc7d82b4 => 1
	i32 3442543374, ; 196: AndHUD => 0xcd310b0e => 15
	i32 3476120550, ; 197: Mono.Android => 0xcf3163e6 => 24
	i32 3485117614, ; 198: System.Text.Json.dll => 0xcfbaacae => 50
	i32 3486566296, ; 199: System.Transactions => 0xcfd0c798 => 107
	i32 3487122080, ; 200: Csv.dll => 0xcfd942a0 => 18
	i32 3501239056, ; 201: Xamarin.AndroidX.AsyncLayoutInflater.dll => 0xd0b0ab10 => 60
	i32 3509114376, ; 202: System.Xml.Linq => 0xd128d608 => 52
	i32 3536029504, ; 203: Xamarin.Forms.Platform.Android.dll => 0xd2c38740 => 100
	i32 3567349600, ; 204: System.ComponentModel.Composition.dll => 0xd4a16f60 => 111
	i32 3608519521, ; 205: System.Linq.dll => 0xd715a361 => 116
	i32 3627220390, ; 206: Xamarin.AndroidX.Print.dll => 0xd832fda6 => 85
	i32 3632359727, ; 207: Xamarin.Forms.Platform => 0xd881692f => 101
	i32 3633644679, ; 208: Xamarin.AndroidX.Annotation.Experimental => 0xd8950487 => 55
	i32 3641597786, ; 209: Xamarin.AndroidX.Lifecycle.LiveData.Core => 0xd90e5f5a => 76
	i32 3672681054, ; 210: Mono.Android.dll => 0xdae8aa5e => 24
	i32 3676310014, ; 211: System.Web.Services.dll => 0xdb2009fe => 112
	i32 3682565725, ; 212: Xamarin.AndroidX.Browser => 0xdb7f7e5d => 61
	i32 3689375977, ; 213: System.Drawing.Common => 0xdbe768e9 => 5
	i32 3718780102, ; 214: Xamarin.AndroidX.Annotation => 0xdda814c6 => 54
	i32 3754567612, ; 215: SQLitePCLRaw.provider.e_sqlite3 => 0xdfca27bc => 38
	i32 3758932259, ; 216: Xamarin.AndroidX.Legacy.Support.V4 => 0xe00cc123 => 74
	i32 3786282454, ; 217: Xamarin.AndroidX.Collection => 0xe1ae15d6 => 63
	i32 3805148751, ; 218: SdkApi.Core => 0xe2cdf64f => 32
	i32 3813669279, ; 219: ApiDatos.dll => 0xe34ff99f => 4
	i32 3822602673, ; 220: Xamarin.AndroidX.Media => 0xe3d849b1 => 83
	i32 3829621856, ; 221: System.Numerics.dll => 0xe4436460 => 44
	i32 3862157298, ; 222: ZebraPrinterSdk => 0xe633d7f2 => 105
	i32 3876362041, ; 223: SQLite-net => 0xe70c9739 => 34
	i32 3885922214, ; 224: Xamarin.AndroidX.Transition.dll => 0xe79e77a6 => 90
	i32 3896760992, ; 225: Xamarin.AndroidX.Core.dll => 0xe843daa0 => 65
	i32 3904602548, ; 226: ViewModel.dll => 0xe8bb81b4 => 3
	i32 3920810846, ; 227: System.IO.Compression.FileSystem.dll => 0xe9b2d35e => 110
	i32 3921031405, ; 228: Xamarin.AndroidX.VersionedParcelable.dll => 0xe9b630ed => 93
	i32 3945713374, ; 229: System.Data.DataSetExtensions.dll => 0xeb2ecede => 108
	i32 3955647286, ; 230: Xamarin.AndroidX.AppCompat.dll => 0xebc66336 => 57
	i32 3998418689, ; 231: SharpSnmpLib.dll => 0xee530701 => 33
	i32 4025784931, ; 232: System.Memory => 0xeff49a63 => 42
	i32 4105002889, ; 233: Mono.Security.dll => 0xf4ad5f89 => 117
	i32 4131741489, ; 234: System.Net.Http.Formatting => 0xf6455f31 => 43
	i32 4151237749, ; 235: System.Core => 0xf76edc75 => 40
	i32 4182413190, ; 236: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 0xf94a8f86 => 80
	i32 4260525087, ; 237: System.Buffers => 0xfdf2741f => 39
	i32 4274976490, ; 238: System.Runtime.Numerics => 0xfecef6ea => 10
	i32 4292120959 ; 239: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 0xffd4917f => 80
], align 4
@assembly_image_cache_indices = local_unnamed_addr constant [240 x i32] [
	i32 23, i32 78, i32 104, i32 27, i32 31, i32 98, i32 14, i32 88, ; 0..7
	i32 88, i32 63, i32 89, i32 61, i32 7, i32 73, i32 11, i32 112, ; 8..15
	i32 77, i32 71, i32 18, i32 53, i32 44, i32 75, i32 37, i32 42, ; 16..23
	i32 97, i32 9, i32 70, i32 25, i32 41, i32 71, i32 82, i32 107, ; 24..31
	i32 115, i32 22, i32 110, i32 67, i32 49, i32 93, i32 58, i32 0, ; 32..39
	i32 52, i32 14, i32 35, i32 32, i32 109, i32 5, i32 85, i32 3, ; 40..47
	i32 11, i32 33, i32 104, i32 27, i32 28, i32 75, i32 20, i32 9, ; 48..55
	i32 87, i32 57, i32 101, i32 79, i32 41, i32 30, i32 31, i32 91, ; 56..63
	i32 26, i32 58, i32 92, i32 16, i32 29, i32 36, i32 69, i32 116, ; 64..71
	i32 113, i32 118, i32 87, i32 83, i32 64, i32 46, i32 115, i32 102, ; 72..79
	i32 109, i32 56, i32 114, i32 68, i32 48, i32 16, i32 81, i32 95, ; 80..87
	i32 66, i32 13, i32 114, i32 0, i32 47, i32 103, i32 7, i32 37, ; 88..95
	i32 4, i32 62, i32 89, i32 40, i32 70, i32 22, i32 81, i32 103, ; 96..103
	i32 12, i32 97, i32 102, i32 59, i32 84, i32 39, i32 79, i32 30, ; 104..111
	i32 76, i32 26, i32 47, i32 45, i32 72, i32 38, i32 99, i32 100, ; 112..119
	i32 23, i32 13, i32 91, i32 82, i32 84, i32 74, i32 86, i32 2, ; 120..127
	i32 54, i32 43, i32 99, i32 8, i32 35, i32 1, i32 21, i32 17, ; 128..135
	i32 108, i32 78, i32 15, i32 49, i32 92, i32 12, i32 17, i32 66, ; 136..143
	i32 77, i32 113, i32 6, i32 10, i32 53, i32 56, i32 98, i32 94, ; 144..151
	i32 28, i32 64, i32 51, i32 94, i32 90, i32 111, i32 25, i32 95, ; 152..159
	i32 45, i32 55, i32 19, i32 69, i32 19, i32 73, i32 20, i32 6, ; 160..167
	i32 48, i32 119, i32 29, i32 106, i32 68, i32 2, i32 117, i32 62, ; 168..175
	i32 119, i32 60, i32 105, i32 34, i32 8, i32 67, i32 106, i32 59, ; 176..183
	i32 72, i32 96, i32 50, i32 36, i32 65, i32 21, i32 86, i32 46, ; 184..191
	i32 118, i32 96, i32 51, i32 1, i32 15, i32 24, i32 50, i32 107, ; 192..199
	i32 18, i32 60, i32 52, i32 100, i32 111, i32 116, i32 85, i32 101, ; 200..207
	i32 55, i32 76, i32 24, i32 112, i32 61, i32 5, i32 54, i32 38, ; 208..215
	i32 74, i32 63, i32 32, i32 4, i32 83, i32 44, i32 105, i32 34, ; 216..223
	i32 90, i32 65, i32 3, i32 110, i32 93, i32 108, i32 57, i32 33, ; 224..231
	i32 42, i32 117, i32 43, i32 40, i32 80, i32 39, i32 10, i32 80 ; 240..239
], align 4

@marshal_methods_number_of_classes = local_unnamed_addr constant i32 0, align 4

; marshal_methods_class_cache
@marshal_methods_class_cache = global [0 x %struct.MarshalMethodsManagedClass] [
], align 4; end of 'marshal_methods_class_cache' array


@get_function_pointer = internal unnamed_addr global void (i32, i32, i32, i8**)* null, align 4

; Function attributes: "frame-pointer"="all" "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+thumb-mode,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" uwtable willreturn writeonly
define void @xamarin_app_init (void (i32, i32, i32, i8**)* %fn) local_unnamed_addr #0
{
	store void (i32, i32, i32, i8**)* %fn, void (i32, i32, i32, i8**)** @get_function_pointer, align 4
	ret void
}

; Names of classes in which marshal methods reside
@mm_class_names = local_unnamed_addr constant [0 x i8*] zeroinitializer, align 4
@__MarshalMethodName_name.0 = internal constant [1 x i8] c"\00", align 1

; mm_method_names
@mm_method_names = local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	; 0
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		i8* getelementptr inbounds ([1 x i8], [1 x i8]* @__MarshalMethodName_name.0, i32 0, i32 0); name
	}
], align 8; end of 'mm_method_names' array


attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable willreturn writeonly "frame-pointer"="all" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+thumb-mode,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" }
attributes #1 = { "min-legal-vector-width"="0" mustprogress "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable "frame-pointer"="all" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+thumb-mode,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" }
attributes #2 = { nounwind }

!llvm.module.flags = !{!0, !1, !2}
!llvm.ident = !{!3}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!2 = !{i32 1, !"min_enum_size", i32 4}
!3 = !{!"Xamarin.Android remotes/origin/d17-5 @ 45b0e144f73b2c8747d8b5ec8cbd3b55beca67f0"}
!llvm.linker.options = !{}
