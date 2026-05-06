// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Global theme toggle behavior for all pages.
(function () {
	var storageKey = "theme-preference";
	var themeToggleButtonId = "theme-toggle";

	function isValidTheme(value) {
		return value === "light" || value === "dark";
	}

	function getStoredTheme() {
		try {
			var storedTheme = localStorage.getItem(storageKey);
			return isValidTheme(storedTheme) ? storedTheme : null;
		} catch (error) {
			return null;
		}
	}

	function setStoredTheme(theme) {
		try {
			localStorage.setItem(storageKey, theme);
		} catch (error) {
			// Ignore storage errors and keep runtime theme only.
		}
	}

	function getSystemTheme() {
		return window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
	}

	function getPreferredTheme() {
		return getStoredTheme() || getSystemTheme();
	}

	function applyTheme(theme) {
		document.documentElement.setAttribute("data-theme", theme);
	}

	function updateToggleState(button, theme) {
		var isDark = theme === "dark";
		button.setAttribute("aria-pressed", isDark.toString());
		button.setAttribute("aria-label", isDark ? "Switch to light mode" : "Switch to dark mode");
		button.textContent = isDark ? "Light mode" : "Dark mode";
	}

	function initializeThemeToggle() {
		var button = document.getElementById(themeToggleButtonId);
		if (!button) {
			return;
		}

		var activeTheme = getPreferredTheme();
		applyTheme(activeTheme);
		updateToggleState(button, activeTheme);

		button.addEventListener("click", function () {
			var currentTheme = document.documentElement.getAttribute("data-theme") === "dark" ? "dark" : "light";
			var nextTheme = currentTheme === "dark" ? "light" : "dark";

			applyTheme(nextTheme);
			setStoredTheme(nextTheme);
			updateToggleState(button, nextTheme);
		});

		if (window.matchMedia) {
			var mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");
			mediaQuery.addEventListener("change", function () {
				if (!getStoredTheme()) {
					var systemTheme = getSystemTheme();
					applyTheme(systemTheme);
					updateToggleState(button, systemTheme);
				}
			});
		}
	}

	if (document.readyState === "loading") {
		document.addEventListener("DOMContentLoaded", initializeThemeToggle);
	} else {
		initializeThemeToggle();
	}
})();
