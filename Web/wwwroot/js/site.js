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

// Decorative background animation with theme-aware shapes.
(function () {
	var canvasId = "background-shapes-canvas";
	var shapeCount = 22;
	var stateStorageKey = "background-shapes-state";
	var paletteByTheme = {
		light: ["rgba(31, 111, 222, 0.32)", "rgba(26, 164, 143, 0.28)", "rgba(207, 113, 57, 0.26)", "rgba(116, 90, 206, 0.24)", "rgba(212, 76, 112, 0.22)", "rgba(54, 141, 180, 0.26)"],
		dark: ["rgba(60, 89, 130, 0.42)", "rgba(62, 102, 95, 0.38)", "rgba(109, 78, 132, 0.34)", "rgba(126, 93, 72, 0.32)", "rgba(74, 88, 124, 0.4)", "rgba(91, 110, 154, 0.34)"]
	};

	function randomBetween(min, max) {
		return min + Math.random() * (max - min);
	}

	function safeSessionGet(key) {
		try {
			return window.sessionStorage.getItem(key);
		} catch (error) {
			return null;
		}
	}

	function safeSessionSet(key, value) {
		try {
			window.sessionStorage.setItem(key, value);
		} catch (error) {
			// Ignore storage failures and keep the animation ephemeral.
		}
	}

	function getTheme() {
		return document.documentElement.getAttribute("data-theme") === "dark" ? "dark" : "light";
	}

	function getViewportSize(canvas) {
		var rect = canvas.getBoundingClientRect();
		return {
			width: Math.max(1, rect.width || window.innerWidth || 1),
			height: Math.max(1, rect.height || window.innerHeight || 1)
		};
	}

	function createShape(width, height) {
		var size = randomBetween(18, 88);
		var direction = Math.random() < 0.5 ? 1 : -1;
		var theme = getTheme();
		var palette = paletteByTheme[theme];

		return {
			type: ["circle", "square", "triangle"][Math.floor(Math.random() * 3)],
			size: size,
			x: direction > 0 ? -randomBetween(size, size * 2.2) : width + randomBetween(size, size * 2.2),
			y: randomBetween(size, Math.max(size, height - size)),
			baseY: 0,
			direction: direction,
			speed: randomBetween(16, 58),
			rotation: randomBetween(0, Math.PI * 2),
			rotationSpeed: randomBetween(-0.7, 0.7),
			waveAmplitude: randomBetween(10, 34),
			waveFrequency: randomBetween(0.003, 0.014),
			wavePhase: randomBetween(0, Math.PI * 2),
			opacity: theme === "dark" ? randomBetween(0.12, 0.24) : randomBetween(0.16, 0.34),
			colorIndex: Math.floor(Math.random() * palette.length)
		};
	}

	function respawnShape(shape, width, height) {
		var direction = Math.random() < 0.5 ? 1 : -1;
		var size = randomBetween(18, 88);
		var theme = getTheme();
		var palette = paletteByTheme[theme];

		shape.type = ["circle", "square", "triangle"][Math.floor(Math.random() * 3)];
		shape.size = size;
		shape.direction = direction;
		shape.x = direction > 0 ? -randomBetween(size, size * 2.5) : width + randomBetween(size, size * 2.5);
		shape.y = randomBetween(size, Math.max(size, height - size));
		shape.baseY = shape.y;
		shape.speed = randomBetween(16, 58);
		shape.rotation = randomBetween(0, Math.PI * 2);
		shape.rotationSpeed = randomBetween(-0.7, 0.7);
		shape.waveAmplitude = randomBetween(10, 34);
		shape.waveFrequency = randomBetween(0.003, 0.014);
		shape.wavePhase = randomBetween(0, Math.PI * 2);
		shape.opacity = theme === "dark" ? randomBetween(0.12, 0.24) : randomBetween(0.16, 0.34);
		shape.colorIndex = Math.floor(Math.random() * palette.length);
	}

	function drawShape(ctx, shape, theme) {
		var palette = paletteByTheme[theme];
		var color = palette[shape.colorIndex % palette.length];
		var half = shape.size / 2;

		ctx.save();
		ctx.translate(shape.x, shape.y);
		ctx.rotate(shape.rotation);
		ctx.globalAlpha = shape.opacity;
		ctx.fillStyle = color;

		if (shape.type === "square") {
			ctx.fillRect(-half, -half, shape.size, shape.size);
		} else if (shape.type === "triangle") {
			ctx.beginPath();
			ctx.moveTo(0, -half);
			ctx.lineTo(half, half);
			ctx.lineTo(-half, half);
			ctx.closePath();
			ctx.fill();
		} else {
			ctx.beginPath();
			ctx.arc(0, 0, half, 0, Math.PI * 2);
			ctx.fill();
		}

		ctx.restore();
	}

	function serializeShape(shape) {
		return {
			type: shape.type,
			size: shape.size,
			x: shape.x,
			y: shape.y,
			baseY: shape.baseY,
			direction: shape.direction,
			speed: shape.speed,
			rotation: shape.rotation,
			rotationSpeed: shape.rotationSpeed,
			waveAmplitude: shape.waveAmplitude,
			waveFrequency: shape.waveFrequency,
			wavePhase: shape.wavePhase,
			opacity: shape.opacity,
			colorIndex: shape.colorIndex
		};
	}

	function restoreShape(rawShape, width, height) {
		if (!rawShape || typeof rawShape !== "object") {
			return null;
		}

		var shape = createShape(width, height);
		shape.type = rawShape.type === "square" || rawShape.type === "triangle" ? rawShape.type : "circle";
		shape.size = Number(rawShape.size) > 0 ? Number(rawShape.size) : shape.size;
		shape.x = Number.isFinite(Number(rawShape.x)) ? Number(rawShape.x) : shape.x;
		shape.y = Number.isFinite(Number(rawShape.y)) ? Number(rawShape.y) : shape.y;
		shape.baseY = Number.isFinite(Number(rawShape.baseY)) ? Number(rawShape.baseY) : shape.y;
		shape.direction = Number(rawShape.direction) < 0 ? -1 : 1;
		shape.speed = Number.isFinite(Number(rawShape.speed)) ? Number(rawShape.speed) : shape.speed;
		shape.rotation = Number.isFinite(Number(rawShape.rotation)) ? Number(rawShape.rotation) : shape.rotation;
		shape.rotationSpeed = Number.isFinite(Number(rawShape.rotationSpeed)) ? Number(rawShape.rotationSpeed) : shape.rotationSpeed;
		shape.waveAmplitude = Number.isFinite(Number(rawShape.waveAmplitude)) ? Number(rawShape.waveAmplitude) : shape.waveAmplitude;
		shape.waveFrequency = Number.isFinite(Number(rawShape.waveFrequency)) ? Number(rawShape.waveFrequency) : shape.waveFrequency;
		shape.wavePhase = Number.isFinite(Number(rawShape.wavePhase)) ? Number(rawShape.wavePhase) : shape.wavePhase;
		shape.opacity = Number.isFinite(Number(rawShape.opacity)) ? Number(rawShape.opacity) : shape.opacity;
		shape.colorIndex = Number.isFinite(Number(rawShape.colorIndex)) ? Number(rawShape.colorIndex) : shape.colorIndex;
		return shape;
	}

	function initializeBackgroundShapes() {
		var canvas = document.getElementById(canvasId);
		if (!canvas || !canvas.getContext) {
			return;
		}

		var ctx = canvas.getContext("2d");
		var shapes = [];
		var reducedMotion = window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches;
		var lastFrameTime = null;
		var width = 0;
		var height = 0;
		var devicePixelRatio = Math.max(1, window.devicePixelRatio || 1);
		var saveThrottle = null;

		function saveState() {
			safeSessionSet(stateStorageKey, JSON.stringify({
				shapes: shapes.map(serializeShape)
			}));
		}

		function scheduleSaveState() {
			if (saveThrottle) {
				window.clearTimeout(saveThrottle);
			}

			saveThrottle = window.setTimeout(function () {
				saveThrottle = null;
				saveState();
			}, 200);
		}

		function loadSavedShapes(nextWidth, nextHeight) {
			var rawState = safeSessionGet(stateStorageKey);
			if (!rawState) {
				return [];
			}

			try {
				var parsedState = JSON.parse(rawState);
				if (!parsedState || !Array.isArray(parsedState.shapes)) {
					return [];
				}

				return parsedState.shapes.slice(0, shapeCount).map(function (rawShape) {
					return restoreShape(rawShape, nextWidth, nextHeight);
				}).filter(Boolean);
			} catch (error) {
				return [];
			}
		}

		function resizeCanvas() {
			var viewport = getViewportSize(canvas);
			width = viewport.width;
			height = viewport.height;
			devicePixelRatio = Math.max(1, window.devicePixelRatio || 1);
			canvas.width = Math.round(width * devicePixelRatio);
			canvas.height = Math.round(height * devicePixelRatio);
			canvas.style.width = width + "px";
			canvas.style.height = height + "px";
			ctx.setTransform(devicePixelRatio, 0, 0, devicePixelRatio, 0, 0);

			if (!shapes.length) {
				var restoredShapes = loadSavedShapes(width, height);
				if (restoredShapes.length) {
					shapes = restoredShapes;
				}

				while (shapes.length < shapeCount) {
					shapes.push(createShape(width, height));
				}
			}

			shapes.forEach(function (shape) {
				shape.y = Math.min(Math.max(shape.y, shape.size), Math.max(shape.size, height - shape.size));
				shape.baseY = shape.y;
			});
		}

		function animate(frameTime) {
			var theme = getTheme();
			var deltaSeconds = lastFrameTime == null ? 0 : Math.min(0.05, (frameTime - lastFrameTime) / 1000);
			var motionScale = reducedMotion ? 0.35 : 1;

			lastFrameTime = frameTime;
			ctx.clearRect(0, 0, width, height);

			shapes.forEach(function (shape) {
				shape.x += shape.speed * shape.direction * deltaSeconds * motionScale;
				shape.rotation += shape.rotationSpeed * deltaSeconds * motionScale;
				shape.y = shape.baseY + Math.sin((shape.x * shape.waveFrequency) + shape.wavePhase) * shape.waveAmplitude;

				if (shape.direction > 0 && shape.x - shape.size > width + 80) {
					respawnShape(shape, width, height);
				} else if (shape.direction < 0 && shape.x + shape.size < -80) {
					respawnShape(shape, width, height);
				}

				drawShape(ctx, shape, theme);
			});

			scheduleSaveState();

			window.requestAnimationFrame(animate);
		}

		resizeCanvas();
		window.addEventListener("resize", resizeCanvas);
		window.addEventListener("pagehide", saveState);
		document.addEventListener("visibilitychange", function () {
			if (document.visibilityState === "hidden") {
				saveState();
			}
		});
		window.requestAnimationFrame(animate);
	}

	if (document.readyState === "loading") {
		document.addEventListener("DOMContentLoaded", initializeBackgroundShapes);
	} else {
		initializeBackgroundShapes();
	}
})();

// Button click burst particles that inherit the clicked button's color.
(function () {
	var particleLayerSelector = ".button-bursts-layer";
	var particleClassName = "button-burst-particle";
	var particleAnimationName = "button-burst-particle-flight";

	function randomBetween(min, max) {
		return min + Math.random() * (max - min);
	}

	function getButtonColor(button) {
		var computedStyle = window.getComputedStyle(button);
		var backgroundColor = computedStyle.backgroundColor;
		var borderColor = computedStyle.borderTopColor || computedStyle.borderColor;
		var textColor = computedStyle.color;

		if (backgroundColor && backgroundColor !== "transparent" && backgroundColor !== "rgba(0, 0, 0, 0)") {
			return backgroundColor;
		}

		if (borderColor && borderColor !== "transparent" && borderColor !== "rgba(0, 0, 0, 0)") {
			return borderColor;
		}

		return textColor || "rgba(120, 140, 170, 0.85)";
	}

	function getParticleLayer() {
		return document.querySelector(particleLayerSelector);
	}

	function emitButtonBurst(button) {
		var layer = getParticleLayer();
		if (!layer) {
			return;
		}

		var bounds = button.getBoundingClientRect();
		if (!bounds.width || !bounds.height) {
			return;
		}

		var reducedMotion = window.matchMedia && window.matchMedia("(prefers-reduced-motion: reduce)").matches;
		var particleTotal = reducedMotion ? 3 : Math.floor(randomBetween(5, 9));
		var color = getButtonColor(button);
		var centerX = bounds.left + (bounds.width / 2);
		var centerY = bounds.top + (bounds.height / 2);

		for (var index = 0; index < particleTotal; index++) {
			var particle = document.createElement("span");
			var angle = randomBetween(0, Math.PI * 2);
			var travel = randomBetween(32, reducedMotion ? 48 : 64);
			var burstX = Math.cos(angle) * travel;
			var burstY = Math.sin(angle) * travel - randomBetween(4, 10);
			var size = randomBetween(11, 18);
			var duration = reducedMotion ? randomBetween(260, 380) : randomBetween(420, 760);

			particle.className = particleClassName;
			particle.style.left = centerX + randomBetween(-6, 6) + "px";
			particle.style.top = centerY + randomBetween(-6, 6) + "px";
			particle.style.width = size + "px";
			particle.style.height = size + "px";
			particle.style.backgroundColor = color;
			particle.style.boxShadow = "0 0 12px color-mix(in srgb, " + color + " 45%, transparent 55%)";
			particle.style.setProperty("--burst-dx", burstX.toFixed(2) + "px");
			particle.style.setProperty("--burst-dy", burstY.toFixed(2) + "px");
			particle.style.animation = particleAnimationName + " " + duration.toFixed(0) + "ms cubic-bezier(0.16, 0.84, 0.36, 1) forwards";

			particle.addEventListener("animationend", function () {
				particle.remove();
			});

			layer.appendChild(particle);

			window.setTimeout(function () {
				if (particle.isConnected) {
					particle.remove();
				}
			}, duration + 120);
		}
	}

	function handlePointerActivation(event) {
		var target = event.target instanceof Element ? event.target.closest("button, .btn") : null;
		if (!target || target.disabled) {
			return;
		}

		emitButtonBurst(target);
	}

	if (document.readyState === "loading") {
		document.addEventListener("DOMContentLoaded", function () {
			document.addEventListener("click", handlePointerActivation, true);
		});
	} else {
		document.addEventListener("click", handlePointerActivation, true);
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

// Culture-aware datepicker with popup calendar and inline validation.
(function () {
	function pad(value) {
		return String(value).padStart(2, "0");
	}

	function getCultureKey(culture) {
		return String(culture || "").toLowerCase();
	}

	function getCultureRules(culture) {
		var normalizedCulture = getCultureKey(culture);
		var isCroatian = normalizedCulture.indexOf("hr") === 0;

		return {
			order: isCroatian ? ["day", "month", "year"] : ["month", "day", "year"],
			separator: isCroatian ? "." : "/",
			firstDayOfWeek: isCroatian ? 1 : 0,
			placeholder: isCroatian ? "dd.MM.yyyy" : "MM/dd/yyyy"
		};
	}

	function isValidDateParts(year, month, day) {
		var candidate = new Date(year, month - 1, day);
		return candidate.getFullYear() === year && candidate.getMonth() === month - 1 && candidate.getDate() === day;
	}

	function parseDate(text, culture) {
		var trimmed = String(text || "").trim().replace(/\.$/, "");
		if (!trimmed) {
			return null;
		}

		if (/^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}/.test(trimmed)) {
			var isoDateTimeParts = trimmed.substring(0, 10).split("-").map(function (part) { return parseInt(part, 10); });
			return isValidDateParts(isoDateTimeParts[0], isoDateTimeParts[1], isoDateTimeParts[2]) ? new Date(isoDateTimeParts[0], isoDateTimeParts[1] - 1, isoDateTimeParts[2]) : null;
		}

		if (/^\d{4}-\d{2}-\d{2}$/.test(trimmed)) {
			var isoParts = trimmed.split("-").map(function (part) { return parseInt(part, 10); });
			return isValidDateParts(isoParts[0], isoParts[1], isoParts[2]) ? new Date(isoParts[0], isoParts[1] - 1, isoParts[2]) : null;
		}

		var rules = getCultureRules(culture);
		var parts = trimmed.split(/[^0-9]+/).filter(Boolean).map(function (part) { return parseInt(part, 10); });
		if (parts.length !== 3 || parts.some(function (part) { return Number.isNaN(part); })) {
			return null;
		}

		var year;
		var month;
		var day;

		switch (rules.order.join("-")) {
			case "day-month-year":
				day = parts[0];
				month = parts[1];
				year = parts[2];
				break;
			default:
				month = parts[0];
				day = parts[1];
				year = parts[2];
				break;
		}

		return isValidDateParts(year, month, day) ? new Date(year, month - 1, day) : null;
	}

	function formatDate(date, culture) {
		if (!(date instanceof Date) || Number.isNaN(date.getTime())) {
			return "";
		}

		var rules = getCultureRules(culture);
		var year = date.getFullYear();
		var month = pad(date.getMonth() + 1);
		var day = pad(date.getDate());

		switch (rules.order.join("-")) {
			case "day-month-year":
				return day + rules.separator + month + rules.separator + year;
			default:
				return month + rules.separator + day + rules.separator + year;
		}
	}

	function formatTime(date) {
		if (!(date instanceof Date) || Number.isNaN(date.getTime())) {
			return "";
		}

		return pad(date.getHours()) + ":" + pad(date.getMinutes());
	}

	function parseTime(text) {
		var trimmed = String(text || "").trim();
		if (!trimmed) {
			return null;
		}

		var match = trimmed.match(/^(\d{1,2}):(\d{2})$/);
		if (!match) {
			return null;
		}

		var hours = parseInt(match[1], 10);
		var minutes = parseInt(match[2], 10);
		if (Number.isNaN(hours) || Number.isNaN(minutes) || hours < 0 || hours > 23 || minutes < 0 || minutes > 59) {
			return null;
		}

		return { hours: hours, minutes: minutes };
	}

	function toIsoDate(date) {
		if (!(date instanceof Date) || Number.isNaN(date.getTime())) {
			return "";
		}

		return date.getFullYear() + "-" + pad(date.getMonth() + 1) + "-" + pad(date.getDate());
	}

	function toIsoDateTime(date, timeText) {
		if (!(date instanceof Date) || Number.isNaN(date.getTime())) {
			return "";
		}

		var parsedTime = parseTime(timeText || formatTime(date));
		if (!parsedTime) {
			return "";
		}

		return toIsoDate(date) + "T" + pad(parsedTime.hours) + ":" + pad(parsedTime.minutes);
	}

	function sameDay(left, right) {
		return !!left && !!right && left.getFullYear() === right.getFullYear() && left.getMonth() === right.getMonth() && left.getDate() === right.getDate();
	}

	function clampViewDate(date) {
		var safeDate = date instanceof Date && !Number.isNaN(date.getTime()) ? date : new Date();
		return new Date(safeDate.getFullYear(), safeDate.getMonth(), 1);
	}

	function enhanceDatepicker(wrapper) {
		if (wrapper.dataset.datepickerEnhanced === "true") {
			return;
		}

		var input = wrapper.querySelector("[data-datepicker-input]");
		var valueField = wrapper.querySelector("[data-datepicker-value]");
		var timeField = wrapper.querySelector("[data-datepicker-time-input]");
		var toggle = wrapper.querySelector("[data-datepicker-toggle]");
		var panel = wrapper.querySelector("[data-datepicker-panel]");
		if (!input || !valueField || !toggle || !panel) {
			return;
		}

		wrapper.dataset.datepickerEnhanced = "true";

		var culture = wrapper.getAttribute("data-culture") || document.documentElement.lang || navigator.language || "en-US";
		var rules = getCultureRules(culture);
		var selectedDate = parseDate(valueField.value, culture) || parseDate(input.value, culture);
		var viewDate = clampViewDate(selectedDate || new Date());
		var isOpen = false;
		var uniqueId = "datepicker-panel-" + Math.random().toString(36).slice(2);
		var form = wrapper.closest("form");

		panel.id = uniqueId;
		toggle.setAttribute("aria-controls", uniqueId);
		toggle.setAttribute("aria-expanded", "false");
		input.setAttribute("aria-haspopup", "dialog");
		input.setAttribute("placeholder", input.getAttribute("placeholder") || rules.placeholder);

		function setValidity(message) {
			input.setCustomValidity(message || "");
			if (timeField) {
				timeField.setCustomValidity(message || "");
			}
			wrapper.classList.toggle("is-invalid", !!message);
		}

		function syncValue(date) {
			if (timeField && !parseTime(timeField.value)) {
				timeField.value = formatTime(date);
			}

			valueField.value = timeField ? toIsoDateTime(date, timeField.value) : toIsoDate(date);
			input.value = formatDate(date, culture);
		}

		function closePanel() {
			if (!isOpen) {
				return;
			}

			isOpen = false;
			panel.hidden = true;
			wrapper.classList.remove("is-open");
			toggle.setAttribute("aria-expanded", "false");
		}

		function renderCalendar() {
			var monthLabel = new Intl.DateTimeFormat(culture, {
				month: "long",
				year: "numeric"
			}).format(viewDate);

			var weekdayFormatter = new Intl.DateTimeFormat(culture, { weekday: "short" });
			var dayFormatter = new Intl.DateTimeFormat(culture, { day: "numeric" });
			var today = new Date();
			var firstDay = new Date(viewDate.getFullYear(), viewDate.getMonth(), 1);
			var daysInMonth = new Date(viewDate.getFullYear(), viewDate.getMonth() + 1, 0).getDate();
			var offset = (firstDay.getDay() - rules.firstDayOfWeek + 7) % 7;
			var gridStart = new Date(firstDay);
			gridStart.setDate(firstDay.getDate() - offset);

			panel.innerHTML = "";

			var header = document.createElement("div");
			header.className = "datepicker-header";

			var previousButton = document.createElement("button");
			previousButton.type = "button";
			previousButton.className = "datepicker-nav";
			previousButton.setAttribute("aria-label", "Previous month");
			previousButton.textContent = "\u2039";

			var title = document.createElement("div");
			title.className = "datepicker-title";
			title.textContent = monthLabel;

			var nextButton = document.createElement("button");
			nextButton.type = "button";
			nextButton.className = "datepicker-nav";
			nextButton.setAttribute("aria-label", "Next month");
			nextButton.textContent = "\u203A";

			previousButton.addEventListener("click", function () {
				viewDate = new Date(viewDate.getFullYear(), viewDate.getMonth() - 1, 1);
				renderCalendar();
			});

			nextButton.addEventListener("click", function () {
				viewDate = new Date(viewDate.getFullYear(), viewDate.getMonth() + 1, 1);
				renderCalendar();
			});

			header.appendChild(previousButton);
			header.appendChild(title);
			header.appendChild(nextButton);

			var weekdayRow = document.createElement("div");
			weekdayRow.className = "datepicker-weekdays";

			for (var i = 0; i < 7; i++) {
				var weekdayIndex = (rules.firstDayOfWeek + i) % 7;
				var sampleDay = new Date(2026, 5, 7 + weekdayIndex);
				var weekdayCell = document.createElement("span");
				weekdayCell.className = "datepicker-weekday";
				weekdayCell.textContent = weekdayFormatter.format(sampleDay);
				weekdayRow.appendChild(weekdayCell);
			}

			var grid = document.createElement("div");
			grid.className = "datepicker-grid";

			for (var dayIndex = 0; dayIndex < 42; dayIndex++) {
				var currentDay = new Date(gridStart.getFullYear(), gridStart.getMonth(), gridStart.getDate() + dayIndex);
				var inCurrentMonth = currentDay.getMonth() === viewDate.getMonth();
				var dayButton = document.createElement("button");
				dayButton.type = "button";
				dayButton.className = "datepicker-day" + (inCurrentMonth ? "" : " is-muted") + (sameDay(currentDay, selectedDate) ? " is-selected" : "") + (sameDay(currentDay, today) ? " is-today" : "");
				dayButton.textContent = dayFormatter.format(currentDay);
				dayButton.setAttribute("aria-label", currentDay.toLocaleDateString(culture, { dateStyle: "full" }));
				dayButton.setAttribute("data-date", currentDay.getFullYear() + "-" + pad(currentDay.getMonth() + 1) + "-" + pad(currentDay.getDate()));
				dayButton.disabled = !inCurrentMonth;

				if (inCurrentMonth) {
					dayButton.addEventListener("click", function (event) {
						var pickedDate = parseDate(event.currentTarget.getAttribute("data-date"), culture);
						if (!pickedDate) {
							return;
						}

						selectedDate = pickedDate;
						viewDate = clampViewDate(selectedDate);
						syncValue(selectedDate);
						setValidity("");
						closePanel();
						input.focus();
					});
				}

				grid.appendChild(dayButton);
			}

			var footer = document.createElement("div");
			footer.className = "datepicker-footer";

			var todayButton = document.createElement("button");
			todayButton.type = "button";
			todayButton.className = "btn btn-sm btn-outline-secondary";
			todayButton.textContent = "Today";
			todayButton.addEventListener("click", function () {
				var now = new Date();
				selectedDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
				viewDate = clampViewDate(selectedDate);
					syncValue(selectedDate);
				setValidity("");
				closePanel();
				input.focus();
			});

			footer.appendChild(todayButton);

			panel.appendChild(header);
			panel.appendChild(weekdayRow);
			panel.appendChild(grid);
			panel.appendChild(footer);
		}

		function openPanel() {
			if (isOpen) {
				return;
			}

			isOpen = true;
			panel.hidden = false;
			wrapper.classList.add("is-open");
			toggle.setAttribute("aria-expanded", "true");
			renderCalendar();
		}

		function validateAndCommit() {
			var trimmedValue = input.value.trim();

			if (!trimmedValue) {
				selectedDate = null;
				valueField.value = "";
				setValidity("Due date is required.");
				return false;
			}

			var parsedDate = parseDate(trimmedValue, culture);
			if (!parsedDate) {
				valueField.value = "";
				setValidity("Enter a valid date.");
				return false;
			}

			if (timeField && !parseTime(timeField.value)) {
				valueField.value = "";
				setValidity("Enter a valid time.");
				return false;
			}

			selectedDate = parsedDate;
			viewDate = clampViewDate(selectedDate);
			syncValue(selectedDate);
			setValidity("");
			return true;
		}

		input.addEventListener("focus", function () {
			openPanel();
		});

		input.addEventListener("input", function () {
			if (!input.value.trim()) {
				valueField.value = "";
			}

			setValidity("");
		});

		if (timeField) {
			timeField.addEventListener("input", function () {
				setValidity("");
			});
		}

		input.addEventListener("blur", function () {
			window.setTimeout(function () {
				validateAndCommit();
			}, 150);
		});

		input.addEventListener("keydown", function (event) {
			if (event.key === "Escape") {
				event.preventDefault();
				closePanel();
				return;
			}

			if (event.key === "ArrowDown") {
				event.preventDefault();
				openPanel();
				return;
			}

			if (event.key === "Enter") {
				if (!validateAndCommit()) {
					event.preventDefault();
					input.reportValidity();
					return;
				}

				closePanel();
			}
		});

		toggle.addEventListener("click", function () {
			if (isOpen) {
				closePanel();
				return;
			}

			openPanel();
			input.focus();
		});

		panel.addEventListener("mousedown", function (event) {
			event.preventDefault();
		});

		panel.addEventListener("click", function (event) {
			event.stopPropagation();
		});

		if (form) {
			form.addEventListener("submit", function (event) {
				if (!validateAndCommit()) {
					event.preventDefault();
					input.reportValidity();
					input.focus();
					return;
				}

				closePanel();
			});
		}

		document.addEventListener("click", function (event) {
			if (!isOpen) {
				return;
			}

			if (wrapper.contains(event.target)) {
				return;
			}

			closePanel();
		});

		if (input.value.trim()) {
			var initialDate = parseDate(input.value, culture);
			if (initialDate) {
				selectedDate = initialDate;
				syncValue(selectedDate);
				setValidity("");
			} else {
				setValidity("Enter a valid date.");
			}
		}
	}

	function enhanceTimeInput(input) {
		if (!input || input.dataset.timepickerEnhanced === "true") return;

		var stepAttr = input.getAttribute("step");
		var minuteStep = 15;
		if (stepAttr) {
			var stepSeconds = parseInt(stepAttr, 10);
			if (!Number.isNaN(stepSeconds) && stepSeconds % 60 === 0) {
				minuteStep = Math.max(1, stepSeconds / 60);
			}
		}

		input.dataset.timepickerEnhanced = "true";

		// Prevent native browser time picker from appearing for inputs of type="time"
		var originalType = input.getAttribute('type') || '';
		if (originalType.toLowerCase() === 'time') {
			input.setAttribute('data-original-type', 'time');
			try { input.type = 'text'; } catch (err) { /* some browsers may restrict changing type */ }
			input.setAttribute('inputmode', 'numeric');
			if (!input.getAttribute('placeholder')) input.setAttribute('placeholder', 'HH:mm');
		}

		var panel = document.createElement("div");
		panel.className = "datepicker-panel timepicker-panel";
		panel.hidden = true;
		panel.setAttribute("role", "dialog");
		panel.setAttribute("aria-hidden", "true");

		function pad(v) { return String(v).padStart(2, "0"); }

		function render() {
			var hoursHtml = "";
			for (var h = 0; h < 24; h++) {
				hoursHtml += '<button type="button" class="timepicker-hour btn">' + pad(h) + '</button>';
			}

			var minutesHtml = "";
			for (var m = 0; m < 60; m += minuteStep) {
				minutesHtml += '<button type="button" class="timepicker-minute btn">' + pad(m) + '</button>';
			}

			panel.innerHTML = '' +
				'<div class="timepicker-header">Select time</div>' +
				'<div class="timepicker-body">' +
				'<div class="timepicker-column timepicker-hours">' + hoursHtml + '</div>' +
				'<div class="timepicker-column timepicker-minutes">' + minutesHtml + '</div>' +
				'</div>';
		}

		render();

		var currentHour = null;

		panel.addEventListener("mousedown", function (e) { e.preventDefault(); });

		panel.addEventListener("click", function (e) {
			e.stopPropagation();
			var btn = e.target.closest("button");
			if (!btn) return;
			if (btn.classList.contains("timepicker-hour")) {
				currentHour = parseInt(btn.textContent, 10);
				var hourBtns = panel.querySelectorAll('.timepicker-hour');
				hourBtns.forEach(function (b) { b.classList.remove('is-selected'); });
				btn.classList.add('is-selected');
				return;
			}
			if (btn.classList.contains("timepicker-minute")) {
				var minute = parseInt(btn.textContent, 10);
				if (currentHour === null) {
					var parsed = (function (txt) { var t = String(txt || '').trim(); var m = t.match(/^(\d{1,2}):(\d{2})$/); if (!m) return null; return { hours: parseInt(m[1],10), minutes: parseInt(m[2],10) }; })(input.value || input.getAttribute('value') || '00:00');
					currentHour = parsed ? parsed.hours : 0;
				}
				input.value = pad(currentHour) + ":" + pad(minute);
				input.dispatchEvent(new Event('input', { bubbles: true }));
				close();
			}
		});

		function open() {
			panel.hidden = false;
			panel.setAttribute('aria-hidden', 'false');
			document.body.appendChild(panel);
			positionPanel();
			setTimeout(function () { document.addEventListener('click', onDocClick); }, 0);
		}

		function close() {
			panel.hidden = true;
			panel.setAttribute('aria-hidden', 'true');
			if (panel.parentNode === document.body) document.body.removeChild(panel);
			document.removeEventListener('click', onDocClick);
		}

		function onDocClick(e) {
			if (e.target === input || panel.contains(e.target)) return;
			close();
		}

		function positionPanel() {
			var rect = input.getBoundingClientRect();
			panel.style.position = 'absolute';
			panel.style.zIndex = 9999;
			panel.style.minWidth = '220px';
			panel.style.left = Math.max(4, rect.left + window.scrollX) + 'px';
			panel.style.top = (rect.bottom + window.scrollY + 6) + 'px';
		}

		input.addEventListener('focus', function () { open(); });
		input.addEventListener('click', function (e) { e.stopPropagation(); if (panel.hidden) open(); });

		input.addEventListener('keydown', function (e) {
			if (e.key === 'Escape') { close(); input.blur(); }
		});

		input.addEventListener('blur', function () {
			setTimeout(function () { if (!panel.contains(document.activeElement)) close(); }, 150);
		});

		var existing = (function (txt) { var t = String(txt||'').trim(); var m = t.match(/^(\d{1,2}):(\d{2})$/); if(!m) return null; return { hours: parseInt(m[1],10), minutes: parseInt(m[2],10)}; })(input.value || input.getAttribute('value') || '');
		if (existing) currentHour = existing.hours;

		input._timepickerPanel = panel;
		input._closeTimepicker = close;
		input._openTimepicker = open;
		}

		function initializeTimepickers() {
			var timeInputs = document.querySelectorAll("input.datepicker-time");
			timeInputs.forEach(function (input) { enhanceTimeInput(input); });
		}

		function initializeDatepickers() {
			var datepickers = document.querySelectorAll("[data-datepicker]");
			datepickers.forEach(function (wrapper) {
				enhanceDatepicker(wrapper);
			});
		}

	if (document.readyState === "loading") {
		document.addEventListener("DOMContentLoaded", function () { initializeDatepickers(); initializeTimepickers(); });
	} else {
		initializeDatepickers(); initializeTimepickers();
	}
})();
