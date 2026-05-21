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

// Lightweight AJAX autocomplete for select fields that need searchable lookups.
(function () {
	function getPlaceholder(select) {
		var customPlaceholder = select.getAttribute("data-autocomplete-placeholder");
		if (customPlaceholder) {
			return customPlaceholder;
		}

		if (select.options.length > 0 && select.options[0].value === "") {
			return select.options[0].textContent.trim();
		}

		return "Search...";
	}

	function getSelectedOption(select) {
		return select.options[select.selectedIndex] || null;
	}

	function getOptionLabel(option) {
		return option ? option.textContent.trim() : "";
	}

	function findExactMatch(select, term, currentResults) {
		var normalizedTerm = term.trim().toLowerCase();
		var result = currentResults.find(function (item) {
			return item.text && item.text.trim().toLowerCase() === normalizedTerm;
		});

		if (result) {
			return result;
		}

		return Array.from(select.options).map(function (option) {
			return {
				value: option.value,
				text: getOptionLabel(option)
			};
		}).find(function (option) {
			return option.value && option.text.toLowerCase() === normalizedTerm;
		}) || null;
	}

	function enhanceSelect(select) {
		if (select.dataset.autocompleteEnhanced === "true") {
			return;
		}

		var sourceUrl = select.getAttribute("data-autocomplete-url");
		if (!sourceUrl) {
			return;
		}

		select.dataset.autocompleteEnhanced = "true";

		var originalId = select.id || ("autocomplete-select-" + Math.random().toString(36).slice(2));
		var originalName = select.name;
		var originalLabel = document.querySelector('label[for="' + originalId + '"]');
		var wrapper = document.createElement("div");
		var input = document.createElement("input");
		var menu = document.createElement("div");
		var valueField = document.createElement("input");
		var debounceHandle = null;
		var requestToken = 0;
		var activeIndex = -1;
		var currentResults = [];
		var committedValue = select.value;
		var committedText = getOptionLabel(getSelectedOption(select));

		wrapper.className = "autocomplete-select";
		input.type = "text";
		input.className = "form-control autocomplete-select-input";
		input.autocomplete = "off";
		input.placeholder = getPlaceholder(select);
		input.value = committedText;
		input.id = originalId;
		input.setAttribute("role", "combobox");
		input.setAttribute("aria-autocomplete", "list");
		input.setAttribute("aria-expanded", "false");

		menu.className = "autocomplete-select-menu";
		menu.id = originalId + "__listbox";
		menu.setAttribute("role", "listbox");
		menu.hidden = true;

		valueField.type = "hidden";
		valueField.name = originalName;
		valueField.value = committedValue;

		if (originalLabel) {
			originalLabel.setAttribute("for", originalId);
		}

		select.removeAttribute("name");
		select.id = originalId + "__source";
		select.classList.add("autocomplete-select-native");
		select.setAttribute("aria-hidden", "true");
		select.setAttribute("tabindex", "-1");
		select.hidden = true;

		select.parentNode.insertBefore(wrapper, select);
		wrapper.appendChild(input);
		wrapper.appendChild(menu);
		wrapper.appendChild(valueField);
		wrapper.appendChild(select);

		function closeMenu() {
			menu.hidden = true;
			input.setAttribute("aria-expanded", "false");
			menu.innerHTML = "";
			currentResults = [];
			activeIndex = -1;
		}

		function setCommittedSelection(value, text) {
			committedValue = String(value);
			committedText = text;
			valueField.value = committedValue;
			select.value = committedValue;
			input.value = committedText;
		}

		function applySelection(item) {
			setCommittedSelection(item.value, item.text);
			closeMenu();
		}

		function renderMenu(items, searchTerm) {
			menu.innerHTML = "";
			activeIndex = -1;

			if (!items.length) {
				var emptyState = document.createElement("div");
				emptyState.className = "autocomplete-select-empty";
				emptyState.textContent = searchTerm ? "No matches found" : "Start typing to search";
				menu.appendChild(emptyState);
			} else {
				items.forEach(function (item, index) {
					var option = document.createElement("button");
					var label = document.createElement("span");
					var hint = document.createElement("span");

					option.type = "button";
					option.className = "autocomplete-select-option";
					option.setAttribute("role", "option");
					option.setAttribute("data-index", String(index));

					label.className = "autocomplete-select-label";
					label.textContent = item.text;
					option.appendChild(label);

					if (item.hint) {
						hint.className = "autocomplete-select-hint";
						hint.textContent = item.hint;
						option.appendChild(hint);
					}

					option.addEventListener("mouseenter", function () {
						activeIndex = index;
						updateActiveItem();
					});

					option.addEventListener("click", function () {
						applySelection(item);
					});

					menu.appendChild(option);
				});
			}

			menu.hidden = false;
			input.setAttribute("aria-expanded", "true");
		}

		function updateActiveItem() {
			var options = menu.querySelectorAll(".autocomplete-select-option");
			options.forEach(function (option, index) {
				option.classList.toggle("is-active", index === activeIndex);
			});
		}

		function fetchResults(searchTerm) {
			var currentToken = ++requestToken;
			var lookupUrl = new URL(sourceUrl, window.location.origin);
			lookupUrl.searchParams.set("limit", select.getAttribute("data-autocomplete-max-results") || "10");

			if (searchTerm) {
				lookupUrl.searchParams.set("q", searchTerm);
			}

			return fetch(lookupUrl.toString(), {
				headers: {
					"Accept": "application/json"
				}
			})
				.then(function (response) {
					if (!response.ok) {
						throw new Error("Autocomplete lookup failed.");
					}

					return response.json();
				})
				.then(function (items) {
					if (currentToken !== requestToken) {
						return;
					}

					currentResults = Array.isArray(items) ? items : [];
					renderMenu(currentResults, searchTerm);
				})
				.catch(function () {
					if (currentToken !== requestToken) {
						return;
					}

					currentResults = [];
					renderMenu([], searchTerm);
				});
		}

		input.addEventListener("input", function () {
			clearTimeout(debounceHandle);
			valueField.value = "";
			select.value = "";

			var searchTerm = input.value.trim();
			if (!searchTerm && committedValue) {
				closeMenu();
				return;
			}

			debounceHandle = window.setTimeout(function () {
				fetchResults(searchTerm);
			}, 180);
		});

		input.addEventListener("focus", function () {
			var searchTerm = input.value.trim();
			fetchResults(searchTerm);
		});

		input.addEventListener("keydown", function (event) {
			var hasVisibleMenu = !menu.hidden;
			var optionCount = currentResults.length;

			if (event.key === "ArrowDown") {
				event.preventDefault();
				if (!hasVisibleMenu) {
					fetchResults(input.value.trim());
					return;
				}

				if (optionCount > 0) {
					activeIndex = Math.min(activeIndex + 1, optionCount - 1);
					updateActiveItem();
				}
				return;
			}

			if (event.key === "ArrowUp") {
				event.preventDefault();
				if (!hasVisibleMenu) {
					fetchResults(input.value.trim());
					return;
				}

				if (optionCount > 0) {
					activeIndex = Math.max(activeIndex - 1, 0);
					updateActiveItem();
				}
				return;
			}

			if (event.key === "Enter") {
				if (hasVisibleMenu && optionCount > 0) {
					event.preventDefault();
					var chosenIndex = activeIndex >= 0 ? activeIndex : 0;
					var chosenItem = currentResults[chosenIndex];
					if (chosenItem) {
						applySelection(chosenItem);
					}
				}
				return;
			}

			if (event.key === "Escape") {
				event.preventDefault();
				setCommittedSelection(committedValue, committedText);
				closeMenu();
			}
		});

		input.addEventListener("blur", function () {
			window.setTimeout(function () {
				var enteredText = input.value.trim();
				if (!enteredText) {
					setCommittedSelection(committedValue, committedText);
					closeMenu();
					return;
				}

				var exactMatch = findExactMatch(select, enteredText, currentResults);
				if (exactMatch) {
					applySelection(exactMatch);
					return;
				}

				setCommittedSelection(committedValue, committedText);
				closeMenu();
			}, 150);
		});

		menu.addEventListener("mousedown", function (event) {
			event.preventDefault();
		});

		if (!committedText) {
			input.value = "";
		}
	}

	function initializeAutocompleteSelects() {
		var selects = document.querySelectorAll("select[data-autocomplete-url]");
		selects.forEach(function (select) {
			enhanceSelect(select);
		});
	}

	if (document.readyState === "loading") {
		document.addEventListener("DOMContentLoaded", initializeAutocompleteSelects);
	} else {
		initializeAutocompleteSelects();
	}
})();
