import { expect, test, type APIResponse, type TestInfo } from '@playwright/test';

test.describe.configure({ mode: 'serial' });

function uniqueLabel(prefix: string, testInfo: TestInfo): string {
    return `${prefix}-${testInfo.project.name}-${testInfo.workerIndex}-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`;
}

async function readJson<T>(response: APIResponse): Promise<T> {
    return (await response.json()) as T;
}

test.describe('API endpoints', () => {
    test('Users API supports the full lifecycle and filtered reads', async ({ request }, testInfo) => {
        const username = uniqueLabel('user', testInfo);
        const email = `${username}@example.com`;
        let createdUserId = 0;

        await test.step('List users and verify data is present', async () => {
            const response = await request.get('/api/users');

            expect(response.status()).toBe(200);

            const users = await readJson<Array<{ id: number; username: string }>>(response);
            expect(Array.isArray(users)).toBe(true);
            expect(users.some((user) => user.id === 1 && user.username === 'ana.kovacic')).toBe(true);
            expect(users.some((user) => user.id === 3)).toBe(true);
        });

        await test.step('Fetch a user by id', async () => {
            const response = await request.get('/api/users/1');

            expect(response.status()).toBe(200);

            const user = await readJson<{ id: number; username: string; email: string }>(response);
            expect(user.id).toBe(1);
            expect(user.username).toBe('ana.kovacic');
            expect(user.email).toBe('ana.kovacic@example.com');
        });

        await test.step('Create a new user', async () => {
            const response = await request.post('/api/users', {
                data: {
                    username,
                    email,
                    passwordHash: 'Password123!Aa',
                    firstName: 'Test',
                    lastName: 'User'
                }
            });

            expect(response.status()).toBe(201);

            const created = await readJson<{ id: number; username: string; email: string; firstName: string; lastName: string }>(response);
            expect(created.username).toBe(username);
            expect(created.email).toBe(email);
            expect(created.firstName).toBe('Test');
            expect(created.lastName).toBe('User');
            createdUserId = created.id;
        });

        await test.step('Fetch the created user by id', async () => {
            const response = await request.get(`/api/users/${createdUserId}`);

            expect(response.status()).toBe(200);

            const user = await readJson<{ id: number; username: string; email: string }>(response);
            expect(user.id).toBe(createdUserId);
            expect(user.username).toBe(username);
            expect(user.email).toBe(email);
        });

        await test.step('Update the created user', async () => {
            const updatedEmail = `${username}.updated@example.com`;
            const response = await request.put(`/api/users/${createdUserId}`, {
                data: {
                    username,
                    email: updatedEmail,
                    passwordHash: 'Password123!Bb',
                    firstName: 'Updated',
                    lastName: 'Person'
                }
            });

            expect(response.status()).toBe(200);

            const user = await readJson<{ id: number; username: string; email: string; firstName: string; lastName: string }>(response);
            expect(user.id).toBe(createdUserId);
            expect(user.username).toBe(username);
            expect(user.email).toBe(updatedEmail);
            expect(user.firstName).toBe('Updated');
            expect(user.lastName).toBe('Person');
        });

        await test.step('Read users with tasks', async () => {
            const response = await request.get('/api/users/with-tasks');

            expect(response.status()).toBe(200);

            const users = await readJson<Array<{ id: number }>>(response);
            expect(users.map((user) => user.id)).toEqual(expect.arrayContaining([1, 2]));
            expect(users.map((user) => user.id)).not.toContain(3);
        });

        await test.step('Read users without tasks', async () => {
            const response = await request.get('/api/users/without-tasks');

            expect(response.status()).toBe(200);

            const users = await readJson<Array<{ id: number }>>(response);
            expect(users.map((user) => user.id)).toContain(3);
            expect(users.map((user) => user.id)).not.toContain(1);
            expect(users.map((user) => user.id)).not.toContain(2);
        });

        await test.step('Delete the created user', async () => {
            const response = await request.delete(`/api/users/${createdUserId}`);

            expect(response.status()).toBe(204);
        });

        await test.step('Confirm the created user is gone', async () => {
            const response = await request.get(`/api/users/${createdUserId}`);

            expect(response.status()).toBe(404);
        });

        await test.step('Reject deleting a user that still has tasks', async () => {
            const response = await request.delete('/api/users/1');

            expect(response.status()).toBe(409);
            expect(await response.text()).toContain('cannot be deleted');
        });
    });

    test('Categories API supports create, update, duplicate checks, and delete', async ({ request }, testInfo) => {
        const categoryName = uniqueLabel('category', testInfo);
        const updatedCategoryName = `${categoryName}-updated`;
        let createdCategoryId = 0;

        await test.step('List categories and verify the seeded catalog', async () => {
            const response = await request.get('/api/categories');

            expect(response.status()).toBe(200);

            const categories = await readJson<Array<{ id: number; name: string }>>(response);
            expect(categories.some((category) => category.id === 1 && category.name === 'Planning')).toBe(true);
        });

        await test.step('Fetch a category by id', async () => {
            const response = await request.get('/api/categories/1');

            expect(response.status()).toBe(200);

            const category = await readJson<{ id: number; name: string; color: string }>(response);
            expect(category.id).toBe(1);
            expect(category.name).toBe('Planning');
            expect(category.color).toBe('#3b82f6');
        });

        await test.step('Create a unique category', async () => {
            const response = await request.post('/api/categories', {
                data: {
                    name: categoryName,
                    description: 'Created by Playwright API tests',
                    color: '#123abc',
                    isActive: true
                }
            });

            expect(response.status()).toBe(201);

            const category = await readJson<{ id: number; name: string; description: string; color: string; isActive: boolean }>(response);
            expect(category.name).toBe(categoryName);
            expect(category.description).toBe('Created by Playwright API tests');
            expect(category.color).toBe('#123abc');
            expect(category.isActive).toBe(true);
            createdCategoryId = category.id;
        });

        await test.step('Fetch the created category', async () => {
            const response = await request.get(`/api/categories/${createdCategoryId}`);

            expect(response.status()).toBe(200);

            const category = await readJson<{ id: number; name: string }>(response);
            expect(category.id).toBe(createdCategoryId);
            expect(category.name).toBe(categoryName);
        });

        await test.step('Update the created category', async () => {
            const response = await request.put(`/api/categories/${createdCategoryId}`, {
                data: {
                    name: updatedCategoryName,
                    description: 'Updated by Playwright API tests',
                    color: '#654321',
                    isActive: false
                }
            });

            expect(response.status()).toBe(200);

            const category = await readJson<{ id: number; name: string; description: string; color: string; isActive: boolean }>(response);
            expect(category.id).toBe(createdCategoryId);
            expect(category.name).toBe(updatedCategoryName);
            expect(category.description).toBe('Updated by Playwright API tests');
            expect(category.color).toBe('#654321');
            expect(category.isActive).toBe(false);
        });

        await test.step('Read back the updated category', async () => {
            const response = await request.get(`/api/categories/${createdCategoryId}`);

            expect(response.status()).toBe(200);

            const category = await readJson<{ id: number; name: string }>(response);
            expect(category.name).toBe(updatedCategoryName);
        });

        await test.step('Reject creating a duplicate category name', async () => {
            const response = await request.post('/api/categories', {
                data: {
                    name: 'Planning',
                    description: 'Duplicate category',
                    color: '#abcdef',
                    isActive: true
                }
            });

            expect(response.status()).toBe(400);
            expect(await response.text()).toContain('already exists');
        });

        await test.step('Delete the created category', async () => {
            const response = await request.delete(`/api/categories/${createdCategoryId}`);

            expect(response.status()).toBe(204);
        });

        await test.step('Confirm the created category is gone', async () => {
            const response = await request.get(`/api/categories/${createdCategoryId}`);

            expect(response.status()).toBe(404);
        });

        await test.step('Reject deleting a seeded category that still has tasks', async () => {
            const response = await request.delete('/api/categories/1');

            expect(response.status()).toBe(409);
            expect(await response.text()).toContain('cannot be deleted');
        });
    });

    test('Tasks API supports CRUD and filtered queries', async ({ request }, testInfo) => {
        const taskTitle = uniqueLabel('task', testInfo);
        const updatedTaskTitle = `${taskTitle}-updated`;
        let createdTaskId = 0;

        const createTaskPayload = {
            title: taskTitle,
            description: 'Created by Playwright API tests',
            dueDate: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(),
            isCompleted: false,
            priorityId: 1,
            userId: 1,
            categoryId: 1
        };

        await test.step('List tasks and verify seeded tasks are present', async () => {
            const response = await request.get('/api/tasks');

            expect(response.status()).toBe(200);

            const tasks = await readJson<Array<{ id: number; title: string }>>(response);
            expect(Array.isArray(tasks)).toBe(true);
            expect(tasks.some((task) => task.id === 1)).toBe(true);
            expect(tasks.length).toBeGreaterThan(0);
        });

        await test.step('Fetch a seeded task by id', async () => {
            const response = await request.get('/api/tasks/1');

            expect(response.status()).toBe(200);

            const task = await readJson<{ id: number; title: string; userId: number; categoryId: number }>(response);
            expect(task.id).toBe(1);
            expect(typeof task.title).toBe('string');
            expect(task.title.length).toBeGreaterThan(0);
            expect(task.userId).toBeGreaterThan(0);
            expect(task.categoryId).toBeGreaterThan(0);
        });

        await test.step('Create a unique task', async () => {
            const response = await request.post('/api/tasks', {
                data: createTaskPayload
            });

            expect(response.status()).toBe(201);

            const task = await readJson<{ id: number; title: string; description: string; isCompleted: boolean }>(response);
            expect(task.title).toBe(taskTitle);
            expect(task.description).toBe('Created by Playwright API tests');
            expect(task.isCompleted).toBe(false);
            createdTaskId = task.id;
        });

        await test.step('Fetch the created task', async () => {
            const response = await request.get(`/api/tasks/${createdTaskId}`);

            expect(response.status()).toBe(200);

            const task = await readJson<{ id: number; title: string; userId: number; categoryId: number }>(response);
            expect(task.id).toBe(createdTaskId);
            expect(task.title).toBe(taskTitle);
            expect(task.userId).toBe(1);
            expect(task.categoryId).toBe(1);
        });

        await test.step('Update the created task', async () => {
            const response = await request.put(`/api/tasks/${createdTaskId}`, {
                data: {
                    title: updatedTaskTitle,
                    description: 'Updated by Playwright API tests',
                    dueDate: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000).toISOString(),
                    isCompleted: true,
                    priorityId: 2,
                    userId: 1,
                    categoryId: 1
                }
            });

            expect(response.status()).toBe(200);

            const task = await readJson<{ id: number; title: string; description: string; isCompleted: boolean }>(response);
            expect(task.id).toBe(createdTaskId);
            expect(task.title).toBe(updatedTaskTitle);
            expect(task.description).toBe('Updated by Playwright API tests');
            expect(task.isCompleted).toBe(true);
        });

        await test.step('Fetch the updated task', async () => {
            const response = await request.get(`/api/tasks/${createdTaskId}`);

            expect(response.status()).toBe(200);

            const task = await readJson<{ id: number; title: string; isCompleted: boolean }>(response);
            expect(task.id).toBe(createdTaskId);
            expect(task.title).toBe(updatedTaskTitle);
            expect(task.isCompleted).toBe(true);
        });

        await test.step('Read completed tasks and confirm the updated task appears', async () => {
            const response = await request.get('/api/tasks/completed');

            expect(response.status()).toBe(200);

            const tasks = await readJson<Array<{ id: number }>>(response);
            expect(Array.isArray(tasks)).toBe(true);
            expect(tasks.length).toBeGreaterThan(0);
            expect(tasks.map((task) => task.id)).toContain(createdTaskId);
        });

        await test.step('Read pending tasks and confirm a seeded pending task is still present', async () => {
            const response = await request.get('/api/tasks/pending');

            expect(response.status()).toBe(200);

            const tasks = await readJson<Array<{ id: number }>>(response);
            expect(Array.isArray(tasks)).toBe(true);
            expect(tasks.length).toBeGreaterThan(0);
        });

        await test.step('Read tasks by category and by user', async () => {
            const byCategoryResponse = await request.get('/api/tasks/category/1');
            const byUserResponse = await request.get('/api/tasks/user/1');

            expect(byCategoryResponse.status()).toBe(200);
            expect(byUserResponse.status()).toBe(200);

            const categoryTasks = await readJson<Array<{ id: number }>>(byCategoryResponse);
            const userTasks = await readJson<Array<{ id: number }>>(byUserResponse);

            expect(Array.isArray(categoryTasks)).toBe(true);
            expect(Array.isArray(userTasks)).toBe(true);
            expect(categoryTasks.length).toBeGreaterThan(0);
            expect(userTasks.length).toBeGreaterThan(0);
            expect(categoryTasks.map((task) => task.id)).toEqual(expect.arrayContaining([1, createdTaskId]));
            expect(userTasks.map((task) => task.id)).toEqual(expect.arrayContaining([1, createdTaskId]));
        });

        await test.step('Delete the created task and confirm it is gone', async () => {
            const deleteResponse = await request.delete(`/api/tasks/${createdTaskId}`);
            const getResponse = await request.get(`/api/tasks/${createdTaskId}`);

            expect(deleteResponse.status()).toBe(204);
            expect(getResponse.status()).toBe(404);
        });
    });

    test('Comments API supports create, update, list-by-task, and delete', async ({ request }, testInfo) => {
        const content = uniqueLabel('comment', testInfo);
        const updatedContent = `${content}-updated`;
        let createdCommentId = 0;

        await test.step('List comments and verify seeded data is present', async () => {
            const response = await request.get('/api/comments');

            expect(response.status()).toBe(200);

            const comments = await readJson<Array<{ id: number; taskItemId: number }>>(response);
            expect(comments.some((comment) => comment.id === 1 && comment.taskItemId === 1)).toBe(true);
        });

        await test.step('Fetch a seeded comment by id', async () => {
            const response = await request.get('/api/comments/1');

            expect(response.status()).toBe(200);

            const comment = await readJson<{ id: number; content: string; taskItemId: number; userId: number }>(response);
            expect(comment.id).toBe(1);
            expect(comment.taskItemId).toBe(1);
            expect(comment.userId).toBe(2);
        });

        await test.step('Create a new comment on a seeded task', async () => {
            const response = await request.post('/api/comments', {
                data: {
                    content,
                    taskItemId: 1,
                    userId: 1
                }
            });

            expect(response.status()).toBe(201);

            const comment = await readJson<{ id: number; content: string; taskItemId: number; userId: number; isEdited: boolean }>(response);
            expect(comment.content).toBe(content);
            expect(comment.taskItemId).toBe(1);
            expect(comment.userId).toBe(1);
            expect(comment.isEdited).toBe(false);
            createdCommentId = comment.id;
        });

        await test.step('Fetch comments for the related task', async () => {
            const response = await request.get('/api/comments/task/1');

            expect(response.status()).toBe(200);

            const comments = await readJson<Array<{ id: number }>>(response);
            expect(comments.map((comment) => comment.id)).toContain(createdCommentId);
        });

        await test.step('Update the created comment', async () => {
            const response = await request.put(`/api/comments/${createdCommentId}`, {
                data: {
                    content: updatedContent,
                    taskItemId: 1,
                    userId: 1
                }
            });

            expect(response.status()).toBe(200);

            const comment = await readJson<{ id: number; content: string; taskItemId: number; userId: number }>(response);
            expect(comment.id).toBe(createdCommentId);
            expect(comment.content).toBe(updatedContent);
            expect(comment.taskItemId).toBe(1);
            expect(comment.userId).toBe(1);
        });

        await test.step('Fetch the updated comment by id', async () => {
            const response = await request.get(`/api/comments/${createdCommentId}`);

            expect(response.status()).toBe(200);

            const comment = await readJson<{ id: number; content: string }>(response);
            expect(comment.id).toBe(createdCommentId);
            expect(comment.content).toBe(updatedContent);
        });

        await test.step('Reject a comment with an unknown task', async () => {
            const response = await request.post('/api/comments', {
                data: {
                    content: 'Invalid comment',
                    taskItemId: 9999,
                    userId: 1
                }
            });

            expect(response.status()).toBe(400);
            expect(await response.text()).toContain('Task 9999 was not found');
        });

        await test.step('Reject a comment with an unknown user', async () => {
            const response = await request.post('/api/comments', {
                data: {
                    content: 'Invalid comment',
                    taskItemId: 1,
                    userId: 9999
                }
            });

            expect(response.status()).toBe(400);
            expect(await response.text()).toContain('User 9999 was not found');
        });

        await test.step('Delete the created comment and confirm it is gone', async () => {
            const deleteResponse = await request.delete(`/api/comments/${createdCommentId}`);
            const getResponse = await request.get(`/api/comments/${createdCommentId}`);

            expect(deleteResponse.status()).toBe(204);
            expect(getResponse.status()).toBe(404);
        });
    });

    test('Attachments API supports create, update, and delete', async ({ request }, testInfo) => {
        const fileName = uniqueLabel('attachment', testInfo);
        const updatedFileName = `${fileName}-updated`;
        let createdAttachmentId = 0;

        await test.step('List attachments and verify seeded data is present', async () => {
            const response = await request.get('/api/attachments');

            expect(response.status()).toBe(200);

            const attachments = await readJson<Array<{ id: number; taskItemId: number }>>(response);
            expect(attachments.some((attachment) => attachment.id === 1 && attachment.taskItemId === 1)).toBe(true);
        });

        await test.step('Fetch a seeded attachment by id', async () => {
            const response = await request.get('/api/attachments/1');

            expect(response.status()).toBe(200);

            const attachment = await readJson<{ id: number; fileName: string; taskItemId: number }>(response);
            expect(attachment.id).toBe(1);
            expect(attachment.fileName).toBe('project-proposal-v2.pdf');
            expect(attachment.taskItemId).toBe(1);
        });

        await test.step('Create a new attachment on a seeded task', async () => {
            const response = await request.post('/api/attachments', {
                data: {
                    fileName,
                    filePath: `/mock-files/${fileName}.txt`,
                    taskItemId: 1
                }
            });

            expect(response.status()).toBe(201);

            const attachment = await readJson<{ id: number; fileName: string; filePath: string; taskItemId: number }>(response);
            expect(attachment.fileName).toBe(fileName);
            expect(attachment.filePath).toBe(`/mock-files/${fileName}.txt`);
            expect(attachment.taskItemId).toBe(1);
            createdAttachmentId = attachment.id;
        });

        await test.step('Fetch the created attachment by id', async () => {
            const response = await request.get(`/api/attachments/${createdAttachmentId}`);

            expect(response.status()).toBe(200);

            const attachment = await readJson<{ id: number; fileName: string }>(response);
            expect(attachment.id).toBe(createdAttachmentId);
            expect(attachment.fileName).toBe(fileName);
        });

        await test.step('Update the created attachment', async () => {
            const response = await request.put(`/api/attachments/${createdAttachmentId}`, {
                data: {
                    fileName: updatedFileName,
                    filePath: `/mock-files/${updatedFileName}.txt`,
                    taskItemId: 1
                }
            });

            expect(response.status()).toBe(200);

            const attachment = await readJson<{ id: number; fileName: string; filePath: string; taskItemId: number }>(response);
            expect(attachment.id).toBe(createdAttachmentId);
            expect(attachment.fileName).toBe(updatedFileName);
            expect(attachment.filePath).toBe(`/mock-files/${updatedFileName}.txt`);
            expect(attachment.taskItemId).toBe(1);
        });

        await test.step('Reject an attachment that points to an unknown task', async () => {
            const response = await request.post('/api/attachments', {
                data: {
                    fileName: 'invalid.txt',
                    filePath: '/mock-files/invalid.txt',
                    taskItemId: 9999
                }
            });

            expect(response.status()).toBe(400);
            expect(await response.text()).toContain('Task 9999 was not found');
        });

        await test.step('Delete the created attachment and confirm it is gone', async () => {
            const deleteResponse = await request.delete(`/api/attachments/${createdAttachmentId}`);
            const getResponse = await request.get(`/api/attachments/${createdAttachmentId}`);

            expect(deleteResponse.status()).toBe(204);
            expect(getResponse.status()).toBe(404);
        });
    });

    test('Histories API supports create, update, and delete', async ({ request }, testInfo) => {
        const action = uniqueLabel('history', testInfo);
        const updatedAction = `${action}-updated`;
        const actionDate = new Date(Date.now() + 3 * 24 * 60 * 60 * 1000).toISOString();
        let createdHistoryId = 0;

        await test.step('List histories and verify seeded data is present', async () => {
            const response = await request.get('/api/histories');

            expect(response.status()).toBe(200);

            const histories = await readJson<Array<{ id: number; taskItemId: number }>>(response);
            expect(histories.some((history) => history.id === 1 && history.taskItemId === 1)).toBe(true);
        });

        await test.step('Fetch a seeded history by id', async () => {
            const response = await request.get('/api/histories/1');

            expect(response.status()).toBe(200);

            const history = await readJson<{ id: number; action: string; taskItemId: number }>(response);
            expect(history.id).toBe(1);
            expect(history.action).toBe('Task created');
            expect(history.taskItemId).toBe(1);
        });

        await test.step('Create a new task history entry', async () => {
            const response = await request.post('/api/histories', {
                data: {
                    action,
                    actionDate,
                    taskItemId: 1
                }
            });

            expect(response.status()).toBe(201);

            const history = await readJson<{ id: number; action: string; actionDate: string; taskItemId: number }>(response);
            expect(history.action).toBe(action);
            expect(history.taskItemId).toBe(1);
            createdHistoryId = history.id;
        });

        await test.step('Fetch the created history by id', async () => {
            const response = await request.get(`/api/histories/${createdHistoryId}`);

            expect(response.status()).toBe(200);

            const history = await readJson<{ id: number; action: string }>(response);
            expect(history.id).toBe(createdHistoryId);
            expect(history.action).toBe(action);
        });

        await test.step('Update the created history entry', async () => {
            const response = await request.put(`/api/histories/${createdHistoryId}`, {
                data: {
                    action: updatedAction,
                    actionDate,
                    taskItemId: 1
                }
            });

            expect(response.status()).toBe(200);

            const history = await readJson<{ id: number; action: string; taskItemId: number }>(response);
            expect(history.id).toBe(createdHistoryId);
            expect(history.action).toBe(updatedAction);
            expect(history.taskItemId).toBe(1);
        });

        await test.step('Reject a history entry for an unknown task', async () => {
            const response = await request.post('/api/histories', {
                data: {
                    action: 'Invalid history',
                    actionDate,
                    taskItemId: 9999
                }
            });

            expect(response.status()).toBe(400);
            expect(await response.text()).toContain('Task 9999 was not found');
        });

        await test.step('Delete the created history entry and confirm it is gone', async () => {
            const deleteResponse = await request.delete(`/api/histories/${createdHistoryId}`);
            const getResponse = await request.get(`/api/histories/${createdHistoryId}`);

            expect(deleteResponse.status()).toBe(204);
            expect(getResponse.status()).toBe(404);
        });
    });
});