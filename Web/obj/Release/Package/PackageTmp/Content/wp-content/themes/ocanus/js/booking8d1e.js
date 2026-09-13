function calculateTotal(price, adults, children, infants) {
	const A = price;

	const totalGuests = adults + children;      // chỉ Adult + Children
	const totalAll    = adults + children + infants;

	let base = 0;   // Giá cơ bản (tour)
	let extra = 0;  // Phụ phí

	// TRƯỜNG HỢP 1: Adults + Children = SỐ CHẴN
	if (totalGuests % 2 === 0) {
		base = A * totalGuests;
		extra += A * 0.35 * infants; // phụ phí infants

	}
	// TRƯỜNG HỢP 2: LẺ & Children > 0
	else if (children > 0) {
		base = A * (totalGuests - 1);   // phần đủ cặp
		extra += A * 0.75;              // 75% trẻ em lẻ
		extra += A * 0.35 * infants;    // infants

	}
	// TRƯỜNG HỢP 3: LẺ & Children = 0
	else {
		base = A * adults;              // chỉ người lớn
		if (totalAll > 1) {
			// Single supplement 40%
			extra += A * 0.40;
		} else {
			// Single supplement 50%
			extra += A * 0.50;
		}

		extra += A * 0.35 * infants;    // infants
	}
	
	extra = Math.ceil(extra);
	
	return {
		base: base,
		extra: extra,
		total: base + extra
	};
}

function saveBookingForm() {
	const form = document.getElementById('booking-form');
	if (!form) return;

	const data = {};
	const formData = new FormData(form);

	formData.forEach((value, key) => {
		data[key] = value;
	});

	localStorage.setItem('booking_form_data', JSON.stringify(data));
}

function updateBookingSummary() {
	const adult = document.querySelector('[name="adult"]');
	const children = document.querySelector('[name="children"]');
	const infant = document.querySelector('[name="infant"]');

	const totalEst = document.querySelector('[name="total_est"]');
	const extraEst = document.querySelector('[name="extra_est"]');
	const depositEst = document.querySelector('[name="deposit_est"]');

	// adult
	if (adult && parseInt(adult.value) !== 0) {
		const p = document.querySelector('.padult');
		if (p) {
			p.classList.add('active');
			const span = p.querySelector('span');
			if (span) span.textContent = adult.value;
		}
	}

	// children
	if (children && parseInt(children.value) !== 0) {
		const p = document.querySelector('.pchildren');
		if (p) {
			p.classList.add('active');
			const span = p.querySelector('span');
			if (span) span.textContent = children.value;
		}
	}

	// infant
	if (infant && parseInt(infant.value) !== 0) {
		const p = document.querySelector('.pinfant');
		if (p) {
			p.classList.add('active');
			const span = p.querySelector('span');
			if (span) span.textContent = infant.value;
		}
	}

	// total
	if (totalEst) {
		const el = document.querySelector('.total-est');
		if (el) el.textContent = 'USD ' + totalEst.value.toLocaleString();
	}

	// extra
	if (extraEst) {
		const el = document.querySelector('.extra-est');
		if (el) el.textContent = 'USD ' + extraEst.value.toLocaleString();
	}

	// deposit
	if (depositEst) {
		const el = document.querySelector('.deposit-est');
		if (el) el.textContent = 'USD ' + depositEst.value.toLocaleString();
	}
}

function restoreBookingForm() {
	const form = document.getElementById('booking-form');
	if (!form) return;

	const saved = localStorage.getItem('booking_form_data');
	if (!saved) return;

	const data = JSON.parse(saved);

	Object.keys(data).forEach(name => {
		if (name === 'action') return;

		const field = form.querySelector(`[name="${name}"]`);
		if (!field) return;

		if (field.type === 'checkbox' || field.type === 'radio') {
			if (field.value === data[name]) {
				field.checked = true;
			}
		} else {
			field.value = data[name];
		}
	});

	// cập nhật summary UI
	updateBookingSummary();
}

function disableField() {
	const params = new URLSearchParams(window.location.search);

	if (params.get('field') === 'disable') {
		const form = document.querySelector('#booking-form');

		const allowFields = ['agree_policy', 'payment_method']; // field được phép edit

		document.querySelectorAll('.participants-info').forEach(el => {
			el.style.pointerEvents = 'none';
			el.style.opacity = '0.6'; // optional: nhìn giống disabled
		});

		form.querySelectorAll('input, textarea, select').forEach(el => {
			const name = el.name;

			// ❗ BỎ QUA nếu nằm trong #payment-section
			if (el.closest('#payment-section')) return;

			// ❗ BỎ QUA nếu là field được phép edit
			if (allowFields.includes(name)) return;

			// xử lý theo loại field
			if (el.tagName === 'SELECT') {
				const hidden = document.createElement('input');
				hidden.type = 'hidden';
				hidden.name = el.name;
				hidden.value = el.value;
				el.after(hidden);

				el.disabled = true;
			} else if (el.type === 'checkbox' || el.type === 'radio') {
				el.disabled = true;

				if (el.checked) {
					const hidden = document.createElement('input');
					hidden.type = 'hidden';
					hidden.name = el.name;
					hidden.value = el.value;
					el.after(hidden);
				}
			} else {
				el.readOnly = true;

				if (el.type === 'date' || el.classList.contains('datepicker')) {
					el.style.pointerEvents = 'none';
				}
			}
		});
	}
}

function customRequiredMessage() {
	const form = document.getElementById('booking-form');
	if (!form) return;

	form.addEventListener('invalid', function (e) {
		setCustomMessage(e.target);
		/*
		e.preventDefault();

		const field = e.target;
		setCustomMessage(field);

		const firstInvalid = form.querySelector(':invalid');
		if (firstInvalid === field) {
			field.focus();
			//field.reportValidity(); // Hiện tooltip của browser
			setTimeout(() => {
				field.reportValidity();
				showing = false;
			}, 1);
		}
		*/
	}, true);

	form.addEventListener('input', function (e) {
		setCustomMessage(e.target);
	});

	form.addEventListener('change', function (e) {
		setCustomMessage(e.target);
	});
}

function setCustomMessage(field) {
	console.log(ocanus_booking.field_required);
	// Reset trước để checkValidity hoạt động đúng
	field.setCustomValidity('');
	if (!field.validity.valid) {
		if (field.validity.valueMissing) {
			field.setCustomValidity(ocanus_booking.field_required);
		} else if (field.validity.typeMismatch && field.type === 'email') {
			field.setCustomValidity(ocanus_booking.email_validate);
		} else if (field.validity.patternMismatch) {
			field.setCustomValidity(ocanus_booking.number_validate);
		}
	}
}

document.addEventListener('DOMContentLoaded', function() {
	restoreBookingForm();
	disableField();
	customRequiredMessage();

	document.querySelectorAll('.participants-info').forEach(element => {
		element.addEventListener('click', function(event) {
			const dropdown = this.nextElementSibling;
			if (dropdown && dropdown.classList.contains('excursion-choose-dropdown')) {
				dropdown.style.display = dropdown.style.display === 'block' ? 'none' : 'block';
			}
		});
	});

	// Xử lý nút đóng dropdown
	document.querySelectorAll('.excursion-choose-dropdown .btn-close').forEach(btn => {
		btn.addEventListener('click', function(event) {
			const participantsInfo = document.querySelector('.participants-info');
			if (participantsInfo) {
				participantsInfo.click();
			}
		});
	});

	// Xử lý nút tăng/giảm số lượng
	document.querySelectorAll('.participants-input').forEach(container => {
		container.addEventListener('click', function(e) {
			const target = e.target;
			if (target.classList.contains('btn-minus') || target.classList.contains('btn-plus')) {
				e.preventDefault();
				const input = target.parentElement.querySelector('input');
				let qty = parseInt(input.value) || 0;

				if (target.classList.contains('btn-minus')) {
					if (qty > 0) qty--;
				} else {
					qty++;
				}

				input.value = qty;

				// Trigger change event
				const event = new Event('change', { bubbles: true });
				input.dispatchEvent(event);
			}
		});

		// Xử lý sự kiện change
		container.addEventListener('change', function(e) {
			if (e.target.tagName === 'INPUT') {
				const input = e.target;
				const form = input.closest('form');
				const val = input.value;
				const name = input.getAttribute('name');

				if (form) {
					const infoElements = form.querySelectorAll(`.participants-info .p${name}`);
					const countElements = form.querySelectorAll(`.participants-info .p${name} .count`);

					if (val === '0') {
						infoElements.forEach(el => {
							el.classList.remove('active');
							el.style.display = 'none';
						});
					} else {
						infoElements.forEach(el => {
							el.classList.add('active');
							el.style.display = '';

							if(val === '1') {
								el.classList.add('is-single');
							} else {
								el.classList.remove('is-single');
							}
						});
					}

					countElements.forEach(el => {
						el.textContent = val;
					});
				}
			}
		});
	});

	document.querySelector('.btn-participants-action').addEventListener('click', async function() {
		const tourType = this.dataset.tourType;
		const tourId   = this.dataset.tourId;

		let price = parseFloat(this.dataset.price) || 100;


		// Lấy giá trị từ các input
		const adults 	= parseInt(document.querySelector('.participant-adult').value) || 0;
		const children 	= parseInt(document.querySelector('.participant-children').value) || 0;
		const infants 	= parseInt(document.querySelector('.participant-baby').value) || 0;


		if (tourType === 'private-tour') {
			const personSend = adults + children;
			const response = await fetch(
				ocanus_booking.ajax_url +
				`?action=get_private_tour_price&tour_id=${tourId}&person=${personSend}`,
				{ method: 'GET' }
			);

			const data = await response.json();

			if (data && data.price) {
				price = parseFloat(data.price);
			}
		}

		const calculate = calculateTotal(price, adults, children, infants);

		// Tính toán tổng tiền
		const total = calculate.total;
		const extra = calculate.extra;
		//const deposit = Math.round(total * 0.05 * 100) / 100;
		const deposit = 200;

		// Hiển thị vào div total-price
		if (tourType === 'private-tour') {
			document.querySelector('.person-est').textContent = 'USD ' + price.toLocaleString();
		}

		document.querySelector('.total-est').textContent = 'USD ' + total.toLocaleString(); // Format số
		document.querySelector('.total-est-input').value = total;

		document.querySelector('.extra-est').textContent = 'USD ' + extra.toLocaleString(); // Format số
		document.querySelector('.extra-est-input').value = extra;

		if (document.querySelectorAll('.deposit-est').length > 0) {
			document.querySelector('.deposit-est').textContent = 'USD ' + deposit.toLocaleString(); // Format số
			document.querySelector('.deposit-est-input').value = deposit;
		}
	});


	if (document.querySelectorAll('.btn-deposit').length > 0) {
		document.querySelector('.btn-deposit').addEventListener('click', function() {
			const form = document.querySelector('#booking-form');
			if (!form.checkValidity()) {
				//e.preventDefault();
				form.reportValidity();
				return;
			}

			const adult         = document.getElementById('adult');
			const participants  = document.getElementById('participants');

			if (adult) {
				if (adult.value === '0') {
					participants.classList.add('focus');
					participants.scrollIntoView({
						behavior: "smooth",
						block: "center"
					});
					return;
				}
			}

			const dateInput = document.getElementById('datepicker');

			if (dateInput) {
				const v = (dateInput.value || '').trim();
				const selectText = (dateInput.dataset.select || '').trim();

				if (!v || (selectText && v === selectText)) {
					dateInput.focus();
					return;
				}
			}

			saveBookingForm();

			const url = new URL(window.location.href);

			url.searchParams.set('action', 'deposit');
			url.searchParams.set('field', 'disable');
			url.hash = 'payment-section';

			window.location.href = url.toString();
		});
	}

	if (document.querySelectorAll('.btn-continue-step-2').length > 0) {
		document.querySelector('.btn-go-back-step-2').addEventListener('click', function () {
			saveBookingForm();

			const url = new URL(window.location.href);

			url.searchParams.delete('action');
			url.searchParams.delete('field');

			window.location.href = url.toString();
		});

		document.querySelector('.btn-continue-step-2').addEventListener('click', function () {
			saveBookingForm();

			if (document.querySelector('input[name="agree_policy"]:checked') === null) {
				alert(document.querySelector('input[name="agree_policy"]').getAttribute('data-content'));
				return;
			}

			if (document.querySelector('input[name="payment_method"]:checked') === null) {
				alert(document.querySelector('input[name="payment_alert_content"]').getAttribute('data-content'));
			} else {
				let paymentType = document.querySelector('input[name="payment_method"]:checked');

				const url = new URL(window.location.href);

				url.searchParams.set('payment', paymentType.value);
				url.hash = 'payment-section';

				window.location.href = url.toString();
			}
		});
	}

	if (document.querySelectorAll('.btn-go-back-step-3').length > 0) {
		document.querySelector('.btn-go-back-step-3').addEventListener('click', function () {
			saveBookingForm();

			const url = new URL(window.location.href);

			url.searchParams.delete('payment');

			window.location.href = url.toString();
		});
	}
});