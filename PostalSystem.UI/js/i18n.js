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
        manual_header: "Manual Entry",
        manual_guid_label: "Tracking GUID",
        manual_guid_placeholder: "Enter Tracking GUID...",
        err_inventory: "Failed to load inventory.",
        err_required: "Please select a package or enter a GUID!",
        fragile_yes: "⚠️ Yes",
        fragile_no: "No",
        transfer_page_title: "Transfer Packages to Your Warehouse",
        transfer_avail_title: "Available to Transfer to Your Warehouse",
        transfer_submit_btn: "TRANSFER",
        transfer_success: "Package transferred successfully! Quack!",
        transit_no_pkgs: "No packages currently in transit.",
        // More to be added
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
        manual_header: "ورود دستی",
        manual_guid_label: "کد رهگیری",
        manual_guid_placeholder: "کد رهگیری را وارد کنید...",
        err_inventory: "بارگذاری موجودی با خطا مواجه شد.",
        err_required: "لطفاً یک بسته را انتخاب کنید یا کد رهگیری را وارد نمایید!",
        fragile_yes: "⚠️ بله",
        fragile_no: "خیر",
        transfer_page_title: "انتقال بسته‌ها به انبار شما",
        transfer_avail_title: "بسته‌های موجود برای انتقال به انبار شما",
        transfer_submit_btn: "انتقال",
        transfer_success: "بسته با موفقیت منتقل شد!",
        transit_no_pkgs: "در حال حاضر هیچ بسته‌ای در مسیر نیست.",
        // More to be added
    }
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