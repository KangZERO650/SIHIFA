**Product Requirement Documents (PRD)**  
 **SIHIFA \- Sistem Informasi Hiburan dan Media Favorit**  
 Aplikasi Desktop Rich UI berbasis WPF, C\#, dan SQLite

 

## **Informasi Dokumen :**

| Nama Produk | SIHIFA — Sistem Informasi Hiburan dan Media Favorit |
| :---- | :---- |
| **Jenis Dokumen** | Product Requirements Document (PRD) |
| **Versi** | 1.1-Draft untuk Pengembangan (Revisi Deskriptif) |
| **Platform** | Desktop Windows (WPF / .NET 8/9) |
| **Teknologi Utama** | C\#, XAML, .NET 8/9, SQLite, EF Core / Dapper, CommunityToolkit.MVVM |
| **Arsitektur** | Pattern MVVM (Model-View-ViewModel) |

 

## **1\. Ringkasan Eksekutif**

SIHIFA (Sistem Informasi Hiburan dan Media Favorit) adalah aplikasi desktop modern berbasis Windows Presentation Foundation (WPF) yang dirancang secara khusus untuk mengelola, mengurasi, dan melacak seluruh koleksi media favorit pengguna. Platform ini mencakup berbagai jenis media, mulai dari anime, film, musik, buku, hingga materi atau catatan pengetahuan yang terpusat dalam satu ruang kerja lokal. Aplikasi ini dibangun dengan filosofi offline-first, di mana kemandirian sistem menjadi prioritas utama. Dengan tidak bergantung pada layanan cloud komersial, SIHIFA memberikan jaminan penuh atas kepemilikan data lokal pengguna, sekaligus menyuguhkan antarmuka yang sangat responsif, modern, dan berkinerja tinggi.

### **1.1 Tujuan Produk**

* **Aplikasi Desktop Terpadu (Rich UI):** Menyediakan antarmuka desktop yang modern, estetis, dan kaya fitur. Ini mencakup navigasi sidebar yang terstruktur dengan baik, kartu ringkasan analitik interaktif, tabel data dengan sistem virtualisasi untuk efisiensi memori, serta kemampuan peralihan tema (dark/light mode) secara instan tanpa mengganggu kenyamanan mata pengguna.


* **Kinerja Native & Offline-First:** Mengoptimalkan penggunaan sumber daya perangkat keras melalui pemanfaatan framework WPF (.NET) dan basis data lokal (SQLite). Hal ini memastikan seluruh data dan fitur utama dapat diakses seketika tanpa memerlukan koneksi internet yang aktif, serta menjaga konsumsi memori (RAM) tetap rendah.  
* **Arsitektur Kode Bersih (MVVM):** Mengimplementasikan pemisahan yang tegas antara logika bisnis, view-model, dan antarmuka pengguna menggunakan arsitektur MVVM (Model-View-ViewModel). Hal ini ditujukan untuk mempermudah proses unit testing, debugging, serta membuka peluang pengembangan berkelanjutan oleh komunitas open-source.  
* **Portabilitas & Manajemen Data Mandiri:** Mendukung fleksibilitas data melalui fitur ekspor dan impor instan. Pengguna dapat dengan mudah membuat salinan cadangan (backup) basis data SQLite mereka, atau mengekspornya ke dalam format universal seperti JSON dan CSV untuk dikelola secara mandiri.

### **1.2 Pernyataan Produk**

SIHIFA secara khusus ditujukan bagi para antusias, kolektor, dan penikmat media yang menginginkan privasi dan kontrol absolut atas preferensi hiburan mereka. Mengingat tingginya risiko eksploitasi data analitik pihak ketiga atau perubahan kebijakan sepihak dari layanan berbayar (SaaS), SIHIFA hadir sebagai alternatif desktop Windows terintegrasi. Produk ini menjanjikan pengalaman pengguna (UX) yang premium, ringan dalam penggunaan sumber daya komputer, beroperasi sepenuhnya secara luring (offline), dan tetap menyediakan alat kurasi visual yang memanjakan mata.

## **2\. Latar Belakang dan Permasalahan**

* **Fragmentasi Data:** Saat ini, preferensi dan riwayat konsumsi media pengguna tersebar secara terpisah di berbagai platform (misalnya: MyAnimeList untuk anime, Letterboxd untuk film, Spotify untuk musik, Goodreads untuk buku, dan Notion untuk catatan materi). Fragmentasi ini menyulitkan pengguna ketika ingin melihat atau mengelola keseluruhan hobi mereka dalam satu dasbor terpadu.  
* **Ketergantungan SaaS & Privasi:** Layanan berbasis daring (cloud) sewaktu-waktu dapat mengubah kebijakan langganan mereka menjadi berbayar, menutup akses API publik, atau memonetisasi data analitik personal pengguna tanpa persetujuan eksplisit. Hal ini mengancam kedaulatan data pengguna atas koleksi pribadi mereka.  
* **Beban Sistem Aplikasi Web:** Penggunaan aplikasi web atau wrapper berbasis browser (seperti Electron) sering kali menjadi 'RAM hog' (memakan memori berlebih) dan membutuhkan koneksi internet yang stabil. Pengguna membutuhkan sebuah aplikasi desktop native Windows yang jauh lebih ringan, memiliki native performance, dan siap digunakan kapan saja (offline).

## **3\. Sasaran, Indikator, dan Batasan**

* **Operasi CRUD & Validasi**: Pengguna harus mampu melakukan manipulasi data (menambah, mengubah, membaca, dan menghapus) dengan sistem validasi form yang jelas. Indikator keberhasilannya adalah tidak adanya crash pada aplikasi saat terjadi kesalahan input pengguna.  
* **Responsivitas Antarmuka:** Aplikasi dirancang untuk merespons input dengan sangat cepat. Indikator keberhasilannya meliputi waktu startup aplikasi yang kurang dari 1,5 detik, serta kemampuan melakukan pencarian instan pada lebih dari 10.000 entri data tanpa jeda lag (respons \< 50 ms).  
* **Integritas Basis Data:** Proses penyimpanan harus konsisten. Indikator keberhasilannya adalah terjaminnya transaksi database ACID oleh engine SQLite lokal, sehingga seluruh perubahan data tersimpan dengan aman tanpa risiko korupsi atau kehilangan data.  
* **Kualitas Rich UI**: Antarmuka harus mempertahankan konsistensi visual. Indikatornya adalah teks yang mudah dibaca dengan kontras yang memadai, komponen yang responsif terhadap ukuran jendela, dan pergantian tema gelap/terang yang berjalan mulus tanpa perlu memulai ulang aplikasi.  
* **Kemudahan Distribusi:** Aplikasi difokuskan pada portabilitas. Indikator keberhasilannya adalah aplikasi dapat didistribusikan dalam bentuk single-file executable (.NET self-contained) atau installer portabel yang tidak memerlukan konfigurasi server yang rumit.

## **4\. Pengguna dan Hak Akses**

| Peran Pengguna | Deskripsi |
| :---- | :---- |
| Pengguna Utama (Personal) | Pengguna perorangan yang memiliki kendali dan hak akses penuh secara lokal. Pengguna ini dapat mengelola seluruh entri media, mengubah preferensi tema UI, dan melakukan manajemen penuh terhadap file database lokal. |
| Sistem Eksternal (Fetcher) | Merupakan worker di latar belakang (background task) yang bersifat opsional. Bertugas mengakses API terbuka (seperti TMDB atau AniList) untuk mengunduh cover poster dan metadata. Hanya memiliki hak Read-Only terhadap jaringan, dan tidak berhak memodifikasi database inti tanpa izin (trigger) pengguna. |

 

## **5\. Ruang Lingkup Produk**

### **5.1 In Scope (Rilis Awal)**

1. Manajemen Media Terintegrasi: Fungsionalitas CRUD penuh untuk multi-entitas (Anime, Film, Musik, Buku, Materi) menggunakan form input yang tervalidasi.  
2. Penyimpanan Lokal: Implementasi SQLite lokal sebagai basis data utama, memanfaatkan framework ORM (EF Core atau Dapper).  
3. Antarmuka Modern: Pembuatan struktur navigasi sidebar, sistem tema (dark/light mode), dan dashboard analitik dinamis (menampilkan total entri, wishlist, dll).  
4. Filter & Pencarian Instan: Penggunaan DataGrid dengan teknik virtualisasi untuk filter instan judul, genre, dan kategori secara real-time.  
5. Portabilitas Data Dasar: Fungsionalitas ekspor/impor file .db SQLite serta kapabilitas backup data ke dalam format JSON/CSV.

### **5.2 Out of Scope**

6\.      Sinkronisasi otomatis atau real-time antar perangkat menggunakan Cloud Database.

7\.      Pengembangan versi aplikasi mobile (Android / iOS) atau web client / portal online.

8\.      Sistem manajemen multi-user bertingkat dengan otentikasi login jarak jauh.

9\.      Fitur pemutar media internal (streaming video/audio langsung di dalam aplikasi).

## **6\. Struktur Navigasi dan Halaman**

| Halaman Utama | Elemen UI | Tujuan / Fungsi |
| :---- | :---- | :---- |
| Dashboard Analytics | Card Statistic (Total Koleksi, Selesai, Wishlist), Grafik Distribusi menggunakan LiveCharts2. | Memberikan visualisasi ringkas dan analitik performa atas koleksi media milik pengguna dalam satu layar penuh. |
| Koleksi & Pustaka Media | Sidebar Filter (Berdasarkan Kategori/Genre), Virtualized DataGrid, Tampilan Card View, Search Bar Real-time. | Sebagai pusat manajemen data; menampilkan, memfilter, dan menelusuri daftar media dengan sangat cepat tanpa delay rendering. |
| Formulir Input/Edit Entitas | Formulir modal/XAML lengkap (Judul, Genre, Rating, Status, Catatan), Tombol 'Fetch from API'. | Memfasilitasi pengguna untuk menambah atau memperbarui detail informasi media, dilengkapi dengan validasi input yang ketat (INotifyDataErrorInfo). |
| Pengaturan (Settings) | Toggle Theme (Dark/Light Mode), Tombol Export/Import Database SQLite, Path Konfigurasi Gambar. | Menyediakan ruang bagi pengguna untuk mengelola preferensi personalisasi antarmuka dan keamanan pencadangan data mereka. |

 

## **7\. Kebutuhan Fitur Desktop (Kebutuhan Fungsional)**

Berikut merupakan detail spesifikasi fitur fungsional desktop berdasarkan prioritas pengembangannya, yang diadaptasi dari rancangan kebutuhan awal SIHIFA:

| Kategori | Fitur | Deskripsi Fungsional WPF | Prioritas |
| :---- | :---- | :---- | :---- |
| Manajemen Media | CRUD Multi-Entitas | Formulir terstruktur berbasis XAML dengan validasi input (INotifyDataErrorInfo) untuk menambah, mengubah, melihat, dan menghapus entri Anime, Film, Musik, Buku, dan Materi. | Must-Have (P0) |
| Penyimpanan Lokal | Local SQLite Engine | Database relasional lokal terintegrasi (SQLite via EF Core / Dapper) yang tersimpan langsung di folder aplikasi/AppData pengguna. | Must-Have (P0) |
| Antarmuka Rich UI | Navigasi Sidebar & Theme Toggle | Tata letak modern dengan Sidebar Navigation menu, responsivitas kontrol visual, dan dukungan peralihan tema Gelap/Terang (Dark/Light Mode). | Must-Have (P0) |
| Pencarian & Filter | Instant Filter & DataGrid | Penelusuran instan judul, kreator, dan kategori menggunakan CollectionViewSource / LINQ filter pada DataGrid atau CardView ter-virtualisasi (smooth scrolling). | Must-Have (P0) |
| Portabilitas Data | Backup & Restore Mandiri | Fitur ekspor/impor berkas database .db SQLite serta serialisasi ke format JSON dan CSV melalui dialog sistem file Windows (Open/SaveFileDialog). | Must-Have (P0) |
| Visualisasi Data | Dashboard Statistik & Grafik | Kartu statistik ringkasan dinamis (Total Entri, Selesai, Wishlist) dan grafik visual distribusi genre/rating menggunakan pustaka grafik native (misal LiveCharts2). | Should-Have (P1) |
| Integrasi Metadata | Asynchronous API Fetcher | Modul latar belakang (async/await HttpClient) untuk menarik sinopsis dan cover poster dari API publik (AniList, TMDB, OpenLibrary) dengan caching gambar lokal. | Should-Have (P1) |
| Peringkat Visual | Tier List Generator | Kanvas visual interaktif (drag-and-drop) untuk menyusun tingkatan favorit (S/A/B/C/D) yang dapat disimpan sebagai file gambar PNG. | Nice-to-Have (P2) |

 

## **8\. User Stories dan Kriteria Penerimaan**

### **US-01 (Input Data Mandiri Offline):**

* Sebagai pengguna, saya ingin dapat memasukkan data entri media secara offline, sehingga koleksi hobi saya tetap terkelola dengan rapi meskipun tidak ada akses internet.  
* Kriteria Penerimaan: Form pop-up muncul untuk input data. Ketika disimpan, data langsung tertulis ke dalam file SQLite secara sinkron. Terdapat mekanisme error handling visual jika field yang wajib (seperti Judul) dibiarkan kosong.

### **US-02 (Pencarian Koleksi Skala Besar):**

* Sebagai pengguna dengan ribuan data, saya ingin melakukan pencarian judul spesifik (misalnya buku atau film) dengan sangat cepat tanpa mengalami lag pada layar.  
* Kriteria Penerimaan: Saat pengguna mengetik di bilah pencarian (Search Bar), sistem memfilter konten DataGrid seketika (\< 50 milidetik respon) berkat implementasi CollectionViewSource di WPF.

### **US-03 (Keamanan & Backup Data):**

* Sebagai kolektor, saya ingin mencadangkan seluruh basis data media saya agar hasil kurasi berbulan-bulan tidak hilang akibat kerusakan sistem operasi.  
* Kriteria Penerimaan: Terdapat tombol "Export Database" yang akan memunculkan dialog Windows (SaveFileDialog). File berformat .db atau .json berhasil disalin ke direktori eksternal pilihan pengguna.

### **US-04 (Kenyamanan Visual / Mode Gelap):**

* Sebagai pengguna yang sering beraktivitas di malam hari, saya ingin mengaktifkan dark mode agar mata tidak cepat lelah saat merapikan koleksi.  
* Kriteria Penerimaan: Terdapat sebuah sakelar (toggle button). Saat diklik, palet warna aplikasi secara otomatis dan instan beralih antara skema gelap dan terang tanpa mewajibkan aplikasi untuk dimuat ulang (restart).

## **9\. Kebutuhan Non-Functional**

| ID | Kategori | Persyaratan Spesifik |
| :---- | :---- | :---- |
| NFR-01 | Performa Antarmuka | Waktu yang dibutuhkan untuk startup aplikasi dari awal harus kurang dari 1,5 detik. Proses scroll pada DataGrid dengan data 10.000+ harus tetap halus (menggunakan UI Virtualization). |
| NFR-02 | Privasi & Keamanan Data | 100% data pengguna harus tersimpan secara lokal di mesin pengguna. Dilarang keras menanamkan skrip pelacakan analitik pengguna (telemetry) ke layanan pihak ketiga manapun. |
| NFR-03 | Integritas Basis Data | Memanfaatkan fitur transaksi bawaan SQLite (ACID compliant) saat melakukan pembaruan massal, agar data tidak korup saat aplikasi tertutup paksa. |
| NFR-04 | Arsitektur Kode & Kerapian | Diwajibkan mengadopsi pola arsitektur MVVM secara disiplin dengan dukungan pustaka CommunityToolkit.MVVM, memisahkan View, ViewModel, Model, dan Service untuk kelayakan Unit Testing. |

 

## **10\. Desain Antarmuka dan Panduan Visual**

* Skema Warna Dinamis: Sistem wajib mendukung Dynamic Resource yang memungkinkan pergantian palet secara real-time. Mode Gelap (Dark Mode) menggunakan warna dasar Arang (Charcoal) atau Slate Navy gelap, sedangkan Mode Terang (Light Mode) menggunakan latar belakang Putih/Abu-abu bersih. Warna aksen (Primary Color) disarankan menggunakan Biru Modern (seperti \#2563EB) atau Ungu elegan untuk menyorot tombol aksi utama.  
* Komponen Tampilan Kaya (Rich UI): Komponen utama mencakup Custom DataGrid yang telah di-styling ulang tanpa border default kaku Windows, penggunaan Card View dengan sudut membulat (rounded corners) dan efek bayangan halus (DropShadow) untuk menampilkan poster film/buku, serta penanda status berupa Badge berwarna (misalnya Hijau untuk 'Completed', Kuning/Jingga untuk 'Plan to Watch').

## **11\. Rancangan Basis Data**

Sistem akan menyimpan data utamanya ke dalam sebuah file SQLite terpadu (misal: 'SIHIFA\_LocalDB.db'). Struktur inti dari tabel pengelola media (Tabel MediaItem) didesain sebagai berikut:

* Id (INTEGER, Primary Key, AUTOINCREMENT)  
* Judul (TEXT, NOT NULL) — Menyimpan nama spesifik media.  
* TipeMedia (TEXT) — Kategori seperti 'Anime', 'Film', 'Buku', 'Musik', 'Materi'.  
* Status (TEXT) — State pengerjaan, misalnya 'Completed', 'Plan to Watch', 'Dropped'.  
* Rating (REAL) — Skala penilaian numerik, misalnya rentang 1.0 hingga 10.0.  
* PathCoverLocal (TEXT) — Direktori path lokal tempat gambar poster disimpan (mencegah blob di dalam DB agar tidak berat).  
* Catatan (TEXT) — Catatan teks tambahan atau impresi dari pengguna terkait karya tersebut.

## **12\. Arsitektur Komponen dan Pola Perangkat Lunak**

| Lapisan Arsitektur | Contoh Modul / Keterangan Implementasi |
| :---- | :---- |
| View Layer (UI/XAML) | MainWindow.xaml (Kerangka utama), MediaListView.xaml (Tampilan daftar). Semua interaksi UI diikat (Binding) secara deklaratif ke ViewModel. |
| ViewModel Layer | MediaViewModel.cs. Menangani State Management. Menggunakan class turunan dari ObservableObject yang disediakan oleh CommunityToolkit.MVVM untuk memicu notifikasi perubahan UI. |
| Model Layer | MediaItem.cs. Merupakan POCO (Plain Old CLR Object) yang memetakan struktur tabel database yang digunakan oleh framework ORM (Entity Framework Core). |
| Service Layer | DatabaseService.cs (Mengelola operasi CRUD ke SQLite) dan ApiFetcherService.cs (Mengelola panggilan jaringan menggunakan HttpClient secara asinkronus). |

 

## **13\. Penanganan Kesalahan dan Validasi Data**

* Validasi Formulir: Menerapkan interface INotifyDataErrorInfo pada setiap ViewModel. Jika pengguna lupa mengisi judul atau memasukkan angka rating di luar batas wajar (misal huruf pada field angka), form akan memberikan tanda merah dan tombol 'Simpan' akan otomatis dinonaktifkan.  
* Exception Handling Koneksi Eksternal: Apabila fitur opsional Fetch API Eksternal (seperti menarik sinopsis dari TMDB) mengalami kegagalan akibat jaringan terputus (Timeout), aplikasi harus menerapkan blok try-catch-finally. Kegagalan ini akan 'ditelan' secara halus tanpa menyebabkan aplikasi crash, dan cukup menampilkan notifikasi toast (popup kecil) yang menyatakan 'Gagal menarik data online, beralih ke mode manual'.

## **14\. Batasan dan Asumsi Pengembangan**

* Asumsi Lingkungan Operasi: Aplikasi dijamin hanya berjalan pada ekosistem Windows, dengan target kompatibilitas spesifik pada Windows 10 dan Windows 11\. Membutuhkan runtime .NET 8 atau .NET 9 yang terpasang pada mesin klien.  
* Batasan Pengelolaan Aset Visual: Semua aset media eksternal (seperti gambar cover yang ditarik dari internet) tidak boleh disimpan langsung ke dalam kolom SQLite sebagai objek BLOB raksasa. Gambar harus diunduh dan disimpan (cache) dalam folder lokal %AppData% milik aplikasi, dan database hanya mencatat path file lokal tersebut.

## **15\. Rencana Pengujian Aplikasi**

* Pengujian CRUD Database (Integrity Test): Melakukan skenario insert, update, pembacaan massal, dan delete ke file lokal SQLite untuk memastikan seluruh operasi mematuhi prinsip ACID.  
* Pengujian Beban UI (Stress Test): Menginjeksikan sekitar 10.000 baris data 'dummy' ke dalam database untuk menguji efektivitas performa CollectionViewSource dan VirtualizingStackPanel saat melakukan proses scrolling.  
* Pengujian Isolasi Fitur Offline: Memutuskan seluruh koneksi internet fisik maupun Wi-Fi pada perangkat, lalu menjalankan alur kerja (workflow) lengkap secara manual. Aplikasi harus tetap beroperasi 100% normal tanpa menunjukkan pesan error sistem (unhandled exception) yang mengganggu.

