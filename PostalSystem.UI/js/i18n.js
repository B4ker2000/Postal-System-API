const translations = {
    en: {
        nav_title: "Postal System",
        nav_back_btn: "← Back",
        nav_logout_btn: "Logout",
        dashboard_title: "Dashboard",
        dashboard_reg_title: "Register Package",
        dashboard_reg_desc: "Start a new shipment and generate a tracking ID.",
        dashboard_reg_btn: "GO TO REGISTRATION",
        dashboard_pickup_title: "Pickup Package",
        dashboard_pickup_desc: "Assign a package from the warehouse to yourself.",
        dashboard_pickup_btn: "PICK UP NOW",
        dashboard_transfer_title: "Transfer Package",
        dashboard_transfer_desc: "Accept a package into your current warehouse.",
        dashboard_transfer_btn: "TRANSFER NOW",
        dashboard_deliver_title: "Confirm Delivery",
        dashboard_deliver_desc: "Mark a package as delivered using the OTP code.",
        dashboard_deliver_btn: "CONFIRM DELIVERY",
        dashboard_cancel_title: "Cancel Package",
        dashboard_cancel_desc: "Flag a package as canceled in the system.",
        dashboard_cancel_btn: "CANCEL PACKAGE",
        dashboard_history_title: "Tracking History",
        dashboard_history_desc: "Search for a package to see its full audit trail.",
        dashboard_history_btn: "SEARCH HISTORY",
        reg_page_title: "Register Package",
        reg_form_header: "Register New Packages",
        reg_city_label: "Destination City",
        reg_city_placeholder: "-- Select Destination City --",
        reg_addr_label: "Destination Address",
        reg_addr_placeholder: "Street, House №, Apartment...",
        reg_fragile_label: "Fragile Package",
        reg_submit_btn: "Register Package",
        reg_err_cities: "Quack! Failed to load cities.",
        reg_err_required: "Please fill in all required fields!",
        reg_success_title: "Package Registered!",
        reg_tracking_id: "Tracking ID:",
        reg_otp: "OTP:",
        reg_view_history: "View Tracking History",
        reg_err_generic: "Something went wrong!",
        pickup_page_title: "Pickup Package",
        pickup_form_header: "Pickup Packages from Your Warehouse",
        pickup_avail_header: "Available in Your Warehouse",
        pickup_submit_btn: "PICK UP",
        pickup_no_pkgs: "No packages available in your warehouse.",
        pickup_success: "Package picked up successfully! Quack!",
        col_guid: "Tracking GUID",
        col_dest: "Destination",
        col_fragile: "Fragile",
        col_postman: "Postman",
        col_warehouse: "Warehouse",
        col_city: "City Name",
        col_timestamp: "Timestamp",
        col_details: "Details",
        manual_header: "Manual Entry",
        manual_guid_label: "Tracking GUID",
        manual_guid_placeholder: "Enter Tracking GUID...",
        err_inventory: "Failed to load inventory.",
        err_required: "Please select a package or enter a GUID!",
        err_guid_otp_required: "Please select a package or enter a GUID and fill the OTP field!",
        err_otp_required: "Please fill the OTP field!",
        err_empty_fields: "Please fill in all fields!",
        err_invalid_credentials: "Invalid login credentials.",
        err_server_connection: "Could not connect to the server. Quack!",
        fragile_yes: "⚠️ Yes",
        fragile_no: "No",
        na_text: "N/A",
        transfer_page_title: "Transfer Packages to Your Warehouse",
        transfer_avail_title: "Available to Transfer to Your Warehouse",
        transfer_submit_btn: "TRANSFER",
        transfer_success: "Package transferred successfully! Quack!",
        transit_no_pkgs: "No packages currently in transit.",
        deliver_page_title: "Deliver a Package You Currently Have",
        deliver_avail_header: "Packages You Currently Have",
        deliver_otp_label: "Delivery OTP",
        deliver_otp_placeholder: "Enter The OTP provided during the package registration...",
        deliver_submit_btn: "DELIVER",
        deliver_no_pkgs: "You currently have no packages.",
        deliver_success: "Package delivered successfully! Quack!",
        cancel_page_title: "Cancel Packages Registered at Your Warehouse",
        cancel_avail_header: "Packages Which can be Cancelled",
        cancel_submit_btn: "CANCEL THIS PACKAGE",
        cancel_no_pkgs: "No packages available to cancel in your warehouse.",
        cancel_success: "Package cancelled successfully! Quack!",
        history_page_title: "Track a package's History",
        history_search_label: "Enter the desired package GUID",
        history_search_btn: "SEARCH",
        history_logs_header: "History Logs",
        history_initial_msg: "Enter a package GUID to see its logs",
        history_no_logs: "No logs found for this package.",
        history_err_not_found: "Package not found or error loading logs.",
        history_success: "Logs retrieved! Quack!",
        login_title: "LOGIN",
        login_label_name: "Postman Name",
        login_placeholder_name: "Enter your name...",
        login_label_phone: "Phone Number",
        login_placeholder_phone: "Enter your phone...",
        login_btn: "SIGN IN",
        login_success: "Login successful! Redirecting..."
    },
    fa: {
        nav_title: "سیستم پستی",
        nav_back_btn: "→ بازگشت",
        nav_logout_btn: "خروج",
        dashboard_title: "داشبورد",
        dashboard_reg_title: "ثبت بسته",
        dashboard_reg_desc: "یک محموله جدید ایجاد کرده و کد رهگیری دریافت کنید.",
        dashboard_reg_btn: "ثبت",
        dashboard_pickup_title: "تحویل گرفتن بسته",
        dashboard_pickup_desc: "بسته ها را از انبار خود صادر کنید.",
        dashboard_pickup_btn: "تحویل بگیر",
        dashboard_transfer_title: "انتقال بسته",
        dashboard_transfer_desc: "یک بسته را به انبار فعلی خود منتقل کنید.",
        dashboard_transfer_btn: "انتقال بده",
        dashboard_deliver_title: "تایید تحویل",
        dashboard_deliver_desc: "بسته را با استفاده از کد OTP به عنوان تحویل شده علامت بزنید.",
        dashboard_deliver_btn: "تأیید تحویل",
        dashboard_cancel_title: "لغو بسته",
        dashboard_cancel_desc: "بسته را در سیستم به عنوان لغو شده علامت بزنید.",
        dashboard_cancel_btn: "لغو بسته",
        dashboard_history_title: "تاریخچه رهگیری",
        dashboard_history_desc: "برای مشاهده تاریخچه کامل، کد بسته را جستجو کنید.",
        dashboard_history_btn: "جستجو در تاریخچه",
        reg_page_title: "ثبت بسته",
        reg_form_header: "ثبت بسته‌های جدید",
        reg_city_label: "شهر مقصد",
        reg_city_placeholder: "-- انتخاب شهر مقصد --",
        reg_addr_label: "آدرس مقصد",
        reg_addr_placeholder: "خیابان، پلاک، واحد...",
        reg_fragile_label: "بسته شکستنی",
        reg_submit_btn: "ثبت بسته",
        reg_err_cities: "کواک! بارگذاری شهرها با خطا مواجه شد.",
        reg_err_required: "لطفاً تمام فیلدهای ضروری را پر کنید!",
        reg_success_title: "بسته با موفقیت ثبت شد!",
        reg_tracking_id: "کد رهگیری:",
        reg_otp: "کد تایید (OTP):",
        reg_view_history: "مشاهده تاریخچه رهگیری",
        reg_err_generic: "خطایی رخ داد!",
        pickup_page_title: "تحویل گرفتن بسته",
        pickup_form_header: "تحویل گرفتن بسته‌ها از انبار شما",
        pickup_avail_header: "موجود در انبار شما",
        pickup_submit_btn: "تحویل بگیر",
        pickup_no_pkgs: "هیچ بسته‌ای در انبار شما موجود نیست.",
        pickup_success: "بسته با موفقیت تحویل گرفته شد! کواک!",
        col_guid: "کد رهگیری",
        col_dest: "مقصد",
        col_fragile: "شکستنی",
        col_postman: "مامور پست",
        col_warehouse: "انبار",
        col_city: "نام شهر",
        col_timestamp: "زمان",
        col_details: "جزئیات",
        manual_header: "ورود دستی",
        manual_guid_label: "کد رهگیری",
        manual_guid_placeholder: "کد رهگیری را وارد کنید...",
        err_inventory: "بارگذاری موجودی با خطا مواجه شد.",
        err_required: "لطفاً یک بسته را انتخاب کنید یا کد رهگیری را وارد نمایید!",
        err_guid_otp_required: "لطفاً یک بسته را انتخاب کنید یا شناسه را وارد کرده و فیلد رمز را پر کنید!",
        err_otp_required: "لطفاً فیلد رمز یک‌بار مصرف را پر کنید!",
        err_empty_fields: "لطفاً تمام فیلدها را پر کنید!",
        err_invalid_credentials: "اطلاعات ورود نادرست است.",
        err_server_connection: "اتصال به سرور برقرار نشد. کواک!",
        fragile_yes: "⚠️ بله",
        fragile_no: "خیر",
        na_text: "نامشخص",
        transfer_page_title: "انتقال بسته‌ها به انبار شما",
        transfer_avail_title: "بسته‌های موجود برای انتقال به انبار شما",
        transfer_submit_btn: "انتقال",
        transfer_success: "بسته با موفقیت منتقل شد!",
        transit_no_pkgs: "در حال حاضر هیچ بسته‌ای در مسیر نیست.",
        deliver_page_title: "تحویل بسته‌ای که در اختیار دارید",
        deliver_avail_header: "بسته‌هایی که در حال حاضر دارید",
        deliver_otp_label: "رمز یک‌بار مصرف تحویل",
        deliver_otp_placeholder: "رمزی را که هنگام ثبت بسته دریافت کرده‌اید وارد کنید...",
        deliver_submit_btn: "تحویل",
        deliver_no_pkgs: "در حال حاضر هیچ بسته‌ای در اختیار ندارید.",
        deliver_success: "بسته با موفقیت تحویل شد! کواک!",
        cancel_page_title: "لغو بسته‌های ثبت شده در انبار شما",
        cancel_avail_header: "بسته‌هایی که قابل لغو هستند",
        cancel_submit_btn: "لغو این بسته",
        cancel_no_pkgs: "هیچ بسته‌ای برای لغو در انبار شما موجود نیست.",
        cancel_success: "بسته با موفقیت لغو شد! کواک!",
        history_page_title: "رهگیری تاریخچه بسته",
        history_search_label: "شناسه بسته مورد نظر را وارد کنید",
        history_search_btn: "جستجو",
        history_logs_header: "گزارشات تاریخچه",
        history_initial_msg: "برای مشاهده گزارشات، شناسه بسته را وارد کنید",
        history_no_logs: "هیچ گزارشی برای این بسته یافت نشد.",
        history_err_not_found: "بسته یافت نشد یا خطایی در بارگذاری گزارشات رخ داد.",
        history_success: "گزارشات با موفقیت دریافت شد! کواک!",
        login_title: "ورود",
        login_label_name: "نام پستچی",
        login_placeholder_name: "نام خود را وارد کنید...",
        login_label_phone: "شماره تلفن",
        login_placeholder_phone: "شماره تلفن خود را وارد کنید...",
        login_btn: "ورود",
        login_success: "ورود موفقیت‌آمیز بود! در حال انتقال..."
    }
};

// Helper to get current language string from dictionary
function t(key) {
    const lang = sessionStorage.getItem('preferredLang') || 'en';
    return translations[lang][key] || key;
};

function updateLanguage(lang) {
    // 1. Save preference
    sessionStorage.setItem('preferredLang', lang);

    // 2. Set direction and lang attribute on HTML tag
    document.documentElement.dir = lang === 'fa' ? 'rtl' : 'ltr';
    document.documentElement.lang = lang;

    // 3. Find all elements with data-i18n attribute and swap text
    const elements = document.querySelectorAll('[data-i18n]');
    elements.forEach(el => {
        const key = el.getAttribute('data-i18n');
        if (translations[lang][key]) {
            el.textContent = translations[lang][key];
        }
    });

    // 4. Handle placeholders
    const placeholderElements = document.querySelectorAll('[data-i18n-placeholder]');
    placeholderElements.forEach(el => {
        const key = el.getAttribute('data-i18n-placeholder');
        if (translations[lang][key]) {
            el.placeholder = translations[lang][key];
        }
    });

    // 5. Handle dynamic fragile cells! (might be expanded upon later if there were other dynamic table texts!)
    const fragileCells = document.querySelectorAll('.fragile-cell');
    fragileCells.forEach(cell => {
        const isFragile = cell.getAttribute('data-fragile') === 'true';
        cell.textContent = isFragile ? translations[lang].fragile_yes : translations[lang].fragile_no;
    });
}

// Initialize language on load
function initLanguage() {
    const savedLang = sessionStorage.getItem('preferredLang') || 'en';
    updateLanguage(savedLang);
}