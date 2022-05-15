window.SwitchTheme = () => {
    let htmlCalssName = document.documentElement.className;
    if (htmlCalssName !== "dark") {
        document.documentElement.classList.add('dark');
    }
    else {
        document.documentElement.classList.remove('dark');
    }
}