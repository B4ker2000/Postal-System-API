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
        reg_title: "Register Package",
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
}

// Initialize language on load
function initLanguage() {
    const savedLang = sessionStorage.getItem('preferredLang') || 'en';
    updateLanguage(savedLang);
}